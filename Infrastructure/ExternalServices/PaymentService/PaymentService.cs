using Application.DTOs.Payment;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System.Text.Json;

namespace Infrastructure.ExternalServices.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PaymentService> _logger;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentService> logger,
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            IUnitOfWork unitOfWork)
        {
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;

            // Configure Stripe
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<ServiceResult<PaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating payment intent for booking {BookingId}", createDto.BookingId);

                // Validate booking exists and is in correct state
                var booking = await _bookingRepository.GetByIdAsync(createDto.BookingId);
                if (booking == null)
                {
                    return ServiceResult<PaymentIntentResponseDto>.Failure("Booking not found");
                }

                if (booking.IsPaid)
                {
                    return ServiceResult<PaymentIntentResponseDto>.Failure("Booking is already paid");
                }

                if (booking.Status != BookingStatus.Confirmed)
                {
                    return ServiceResult<PaymentIntentResponseDto>.Failure("Booking must be confirmed before payment");
                }

                // Create payment intent with Stripe
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(createDto.Amount * 100), // Convert to cents
                    Currency = createDto.Currency.ToLower(),
                    Description = createDto.Description ?? $"Payment for booking #{createDto.BookingId}",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        ["booking_id"] = createDto.BookingId.ToString(),
                        ["passenger_id"] = booking.PassengerId,
                        ["trip_id"] = booking.TripId.ToString()
                    }
                };

                // Add customer information if provided
                if (!string.IsNullOrEmpty(createDto.CustomerEmail))
                {
                    options.ReceiptEmail = createDto.CustomerEmail;
                }

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                var response = new PaymentIntentResponseDto
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.Id,
                    Amount = createDto.Amount,
                    Currency = createDto.Currency,
                    Status = paymentIntent.Status,
                    BookingId = createDto.BookingId
                };

                _logger.LogInformation("Payment intent created successfully: {PaymentIntentId}", paymentIntent.Id);
                return ServiceResult<PaymentIntentResponseDto>.Success(response);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating payment intent for booking {BookingId}", createDto.BookingId);
                return ServiceResult<PaymentIntentResponseDto>.Failure($"Payment processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent for booking {BookingId}", createDto.BookingId);
                return ServiceResult<PaymentIntentResponseDto>.Failure("An error occurred while processing payment");
            }
        }

        public async Task<ServiceResult<PaymentResultDto>> ConfirmPaymentAsync(PaymentConfirmationDto confirmationDto)
        {
            try
            {
                _logger.LogInformation("Confirming payment {PaymentIntentId} for booking {BookingId}", 
                    confirmationDto.PaymentIntentId, confirmationDto.BookingId);

                // Get payment intent from Stripe
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(confirmationDto.PaymentIntentId);

                if (paymentIntent == null)
                {
                    return ServiceResult<PaymentResultDto>.Failure("Payment intent not found");
                }

                // Verify booking ID matches
                if (!paymentIntent.Metadata.TryGetValue("booking_id", out var bookingIdStr) ||
                    !int.TryParse(bookingIdStr, out var bookingId) ||
                    bookingId != confirmationDto.BookingId)
                {
                    return ServiceResult<PaymentResultDto>.Failure("Payment intent does not match booking");
                }

                var result = new PaymentResultDto
                {
                    PaymentIntentId = paymentIntent.Id,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount / 100m, // Convert from cents
                    Currency = paymentIntent.Currency,
                    PaymentDate = DateTime.UtcNow,
                    BookingId = confirmationDto.BookingId
                };

                // Check payment status
                if (paymentIntent.Status == "succeeded")
                {
                    // Update booking and generate tickets
                    var updateResult = await UpdateBookingAfterPaymentAsync(confirmationDto.BookingId, paymentIntent.Id);
                    if (!updateResult.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = string.Join(", ", updateResult.Errors);
                        return ServiceResult<PaymentResultDto>.Success(result);
                    }

                    var ticketsResult = await GenerateTicketsAsync(confirmationDto.BookingId);
                    if (ticketsResult.IsSuccess)
                    {
                        result.TicketIds = ticketsResult.Data ?? new List<int>();
                    }

                    result.IsSuccess = true;
                    _logger.LogInformation("Payment confirmed successfully for booking {BookingId}", confirmationDto.BookingId);
                }
                else
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = $"Payment status: {paymentIntent.Status}";
                }

                return ServiceResult<PaymentResultDto>.Success(result);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error confirming payment {PaymentIntentId}", confirmationDto.PaymentIntentId);
                return ServiceResult<PaymentResultDto>.Failure($"Payment confirmation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment {PaymentIntentId}", confirmationDto.PaymentIntentId);
                return ServiceResult<PaymentResultDto>.Failure("An error occurred while confirming payment");
            }
        }

        public async Task<ServiceResult<PaymentResultDto>> GetPaymentStatusAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                if (paymentIntent == null)
                {
                    return ServiceResult<PaymentResultDto>.Failure("Payment intent not found");
                }

                var result = new PaymentResultDto
                {
                    IsSuccess = paymentIntent.Status == "succeeded",
                    PaymentIntentId = paymentIntent.Id,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount / 100m,
                    Currency = paymentIntent.Currency,
                    PaymentDate = paymentIntent.Created
                };

                if (paymentIntent.Metadata.TryGetValue("booking_id", out var bookingIdStr) &&
                    int.TryParse(bookingIdStr, out var bookingId))
                {
                    result.BookingId = bookingId;
                }

                return ServiceResult<PaymentResultDto>.Success(result);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error getting payment status {PaymentIntentId}", paymentIntentId);
                return ServiceResult<PaymentResultDto>.Failure($"Error retrieving payment status: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status {PaymentIntentId}", paymentIntentId);
                return ServiceResult<PaymentResultDto>.Failure("An error occurred while retrieving payment status");
            }
        }

        public async Task<ServiceResult<RefundResultDto>> ProcessRefundAsync(RefundDto refundDto)
        {
            try
            {
                _logger.LogInformation("Processing refund for payment {PaymentIntentId}", refundDto.PaymentIntentId);

                // Get payment intent to get charge ID
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(refundDto.PaymentIntentId);

                if (paymentIntent == null || paymentIntent.Status != "succeeded")
                {
                    return ServiceResult<RefundResultDto>.Failure("Payment intent not found or not succeeded");
                }

                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntent.Id,
                    Reason = refundDto.Reason switch
                    {
                        "duplicate" => "duplicate",
                        "fraudulent" => "fraudulent",
                        _ => "requested_by_customer"
                    }
                };

                if (refundDto.Amount.HasValue)
                {
                    options.Amount = (long)(refundDto.Amount.Value * 100); // Convert to cents
                }

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(options);

                var result = new RefundResultDto
                {
                    IsSuccess = refund.Status == "succeeded",
                    RefundId = refund.Id,
                    Amount = refund.Amount / 100m,
                    Status = refund.Status
                };

                _logger.LogInformation("Refund processed: {RefundId} with status {Status}", refund.Id, refund.Status);
                return ServiceResult<RefundResultDto>.Success(result);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error processing refund for payment {PaymentIntentId}", refundDto.PaymentIntentId);
                return ServiceResult<RefundResultDto>.Failure($"Refund processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment {PaymentIntentId}", refundDto.PaymentIntentId);
                return ServiceResult<RefundResultDto>.Failure("An error occurred while processing refund");
            }
        }

        public async Task<ServiceResult> HandleWebhookAsync(string payload, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(payload, signature, _stripeSettings.WebhookSecret);
                
                _logger.LogInformation("Received webhook event: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await HandlePaymentSucceededAsync(stripeEvent);
                        break;
                    case "payment_intent.payment_failed":
                        await HandlePaymentFailedAsync(stripeEvent);
                        break;
                    default:
                        _logger.LogInformation("Unhandled webhook event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return ServiceResult.Success();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error");
                return ServiceResult.Failure($"Webhook processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return ServiceResult.Failure("An error occurred while processing webhook");
            }
        }

        public async Task<ServiceResult> UpdateBookingAfterPaymentAsync(int bookingId, string paymentIntentId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return ServiceResult.Failure("Booking not found");
                }

                booking.IsPaid = true;
                booking.Status = BookingStatus.Confirmed;
                
                _bookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Booking {BookingId} updated after successful payment {PaymentIntentId}", 
                    bookingId, paymentIntentId);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId} after payment", bookingId);
                return ServiceResult.Failure("Failed to update booking after payment");
            }
        }

        public async Task<ServiceResult<List<int>>> GenerateTicketsAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return ServiceResult<List<int>>.Failure("Booking not found");
                }

                var ticketIds = new List<int>();

                for (int i = 0; i < booking.NumberOfTickets; i++)
                {
                    var ticket = new Ticket
                    {
                        BookingId = bookingId,
                        TripId = booking.TripId,
                        PassengerId = booking.PassengerId,
                        TicketCode = GenerateTicketNumber(),
                        PurchaseDate = DateTime.UtcNow,
                        IsUsed = false,
                        SeatNumber = null, // Can be assigned later
                        Price = booking.TotalPrice / booking.NumberOfTickets
                    };

                    await _ticketRepository.AddAsync(ticket);
                    ticketIds.Add(ticket.Id);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Generated {Count} tickets for booking {BookingId}", 
                    booking.NumberOfTickets, bookingId);

                return ServiceResult<List<int>>.Success(ticketIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tickets for booking {BookingId}", bookingId);
                return ServiceResult<List<int>>.Failure("Failed to generate tickets");
            }
        }

        private async Task HandlePaymentSucceededAsync(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent?.Metadata.TryGetValue("booking_id", out var bookingIdStr) == true &&
                    int.TryParse(bookingIdStr, out var bookingId))
                {
                    await UpdateBookingAfterPaymentAsync(bookingId, paymentIntent.Id);
                    await GenerateTicketsAsync(bookingId);
                    
                    _logger.LogInformation("Payment succeeded webhook processed for booking {BookingId}", bookingId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment succeeded webhook");
            }
        }

        private async Task HandlePaymentFailedAsync(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent?.Metadata.TryGetValue("booking_id", out var bookingIdStr) == true &&
                    int.TryParse(bookingIdStr, out var bookingId))
                {
                    var booking = await _bookingRepository.GetByIdAsync(bookingId);
                    if (booking != null)
                    {
                        booking.Status = BookingStatus.Cancelled;
                        _bookingRepository.Update(booking);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    
                    _logger.LogInformation("Payment failed webhook processed for booking {BookingId}", bookingId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment failed webhook");
            }
        }

        private string GenerateTicketNumber()
        {
            return $"TKT{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}