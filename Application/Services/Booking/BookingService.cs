using Application.DTOs.Booking;
using Application.DTOs.Payment;
using Application.DTOs.Ticket;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<BookingService> _logger;
        private readonly Random _random;

        public BookingService(
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            ILogger<BookingService> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _logger = logger;
            _random = new Random();
        }

        public async Task<ServiceResult<BookingDto>> CreateBookingAsync(
            CreateBookingDto createDto, string passengerId)
        {
            try
            {
                if (createDto.StartStationId == createDto.EndStationId)
                    return ServiceResult<BookingDto>.Failure("Start and end stations cannot be the same");
                if (createDto.NumberOfTickets <= 0)
                    return ServiceResult<BookingDto>.Failure("Number of tickets must be greater than zero");

                var trip = await _unitOfWork.Trips.GetTripByIdWithDetailsAsync(createDto.TripId);
                if (trip == null)
                    return ServiceResult<BookingDto>.Failure($"Trip {createDto.TripId} not found");
                if (trip.AvailableSeats < createDto.NumberOfTickets)
                    return ServiceResult<BookingDto>.Failure(
                        $"Not enough seats. Available: {trip.AvailableSeats}");

                var stations = trip.TripStations.OrderBy(ts => ts.SequenceNumber).ToList();
                var start = stations.FirstOrDefault(ts => ts.StationId == createDto.StartStationId);
                var end = stations.FirstOrDefault(ts => ts.StationId == createDto.EndStationId);

                if (start == null) return ServiceResult<BookingDto>.Failure($"Start station {createDto.StartStationId} not in trip");
                if (end == null) return ServiceResult<BookingDto>.Failure($"End station {createDto.EndStationId} not in trip");
                if (start.SequenceNumber >= end.SequenceNumber)
                    return ServiceResult<BookingDto>.Failure("Start must come before end station");

                var passenger = await _unitOfWork.Users.GetPassengerByUserIdAsync(passengerId);
                if (passenger == null)
                    return ServiceResult<BookingDto>.Failure($"Passenger {passengerId} not found");

                var booking = new Domain.Entities.Booking
                {
                    PassengerId = passengerId,
                    TripId = createDto.TripId,
                    StartStationId = createDto.StartStationId,
                    EndStationId = createDto.EndStationId,
                    BookingDate = DateTime.UtcNow,
                    TotalPrice = trip.Price * createDto.NumberOfTickets,
                    Status = BookingStatus.Pending,
                    IsPaid = false,
                    NumberOfTickets = createDto.NumberOfTickets
                };

                await _unitOfWork.Bookings.AddAsync(booking);
                trip.AvailableSeats -= createDto.NumberOfTickets;
                _unitOfWork.Trips.Update(trip);
                await _unitOfWork.SaveChangesAsync();

                var created = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
                return ServiceResult<BookingDto>.Success(MapBookingToDto(created!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return ServiceResult<BookingDto>.Failure($"Error creating booking: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BookingDto>> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
                if (booking == null)
                    return ServiceResult<BookingDto>.Failure($"Booking {bookingId} not found");
                return ServiceResult<BookingDto>.Success(MapBookingToDto(booking));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {BookingId}", bookingId);
                return ServiceResult<BookingDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PassengerBookingsResponseDto>> GetPassengerBookingsAsync(
            string passengerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var paged = await _unitOfWork.Bookings
                    .GetPaginatedBookingsByPassengerIdAsync(passengerId, pageNumber, pageSize);

                // Debug log — remove after confirming fix
                foreach (var b in paged.Bookings)
                    _logger.LogDebug("Booking {Id}: start={Start}|{StartCity} end={End}|{EndCity}",
                        b.Id,
                        b.StartStation?.Name ?? "NULL", b.StartStation?.City?.Name ?? "NULL",
                        b.EndStation?.Name ?? "NULL", b.EndStation?.City?.Name ?? "NULL");

                return ServiceResult<PassengerBookingsResponseDto>.Success(new PassengerBookingsResponseDto
                {
                    TotalCount = paged.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Bookings = paged.Bookings.Select(MapBookingToDto).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for passenger {PassengerId}", passengerId);
                return ServiceResult<PassengerBookingsResponseDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BookingConfirmationDto>> ProcessPaymentAsync(
            ProcessPaymentDto paymentDto, string passengerId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(paymentDto.BookingId);
                if (booking == null)
                    return ServiceResult<BookingConfirmationDto>.Failure($"Booking {paymentDto.BookingId} not found");
                if (booking.PassengerId != passengerId)
                    return ServiceResult<BookingConfirmationDto>.Failure("No permission");
                if (booking.IsPaid)
                    return ServiceResult<BookingConfirmationDto>.Failure("Already paid");
                if (booking.Status == BookingStatus.Cancelled)
                    return ServiceResult<BookingConfirmationDto>.Failure("Booking is cancelled");

                var paymentResult = await _paymentService.CreatePaymentIntentAsync(new CreatePaymentIntentDto
                {
                    BookingId = paymentDto.BookingId,
                    Amount = booking.TotalPrice,
                    Currency = "usd",
                    Description = $"Ra7ala Booking #{paymentDto.BookingId}",
                    CustomerEmail = paymentDto.CustomerEmail,
                    CustomerName = paymentDto.CustomerName
                });

                if (!paymentResult.IsSuccess)
                    return ServiceResult<BookingConfirmationDto>.Failure(
                        "Payment failed: " + string.Join(", ", paymentResult.Errors));

                return ServiceResult<BookingConfirmationDto>.Success(new BookingConfirmationDto
                {
                    BookingId = booking.Id,
                    Success = false,
                    TotalPrice = booking.TotalPrice,
                    PaymentStatus = "requires_payment_method",
                    PaymentIntentId = paymentResult.Data!.PaymentIntentId,
                    ClientSecret = paymentResult.Data.ClientSecret,
                    Tickets = new List<TicketDto>(),
                    Message = "Use clientSecret with Stripe.js then call /api/Payments/confirm-payment"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for booking {BookingId}", paymentDto.BookingId);
                return ServiceResult<BookingConfirmationDto>.Failure("Error processing payment");
            }
        }

        public async Task<ServiceResult> CancelBookingAsync(int bookingId, string passengerId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
                if (booking == null) return ServiceResult.Failure($"Booking {bookingId} not found");
                if (booking.PassengerId != passengerId) return ServiceResult.Failure("No permission");
                if (booking.Status == BookingStatus.Cancelled) return ServiceResult.Failure("Already cancelled");
                if (booking.IsPaid && booking.Tickets.Any(t => t.IsUsed))
                    return ServiceResult.Failure("Cannot cancel — tickets already used");

                if (booking.IsPaid && !string.IsNullOrEmpty(booking.PaymentIntentId))
                {
                    var refund = await _paymentService.ProcessRefundAsync(new RefundDto
                    {
                        PaymentIntentId = booking.PaymentIntentId,
                        Reason = "requested_by_customer"
                    });
                    if (!refund.IsSuccess)
                        return ServiceResult.Failure("Refund failed: " + string.Join(", ", refund.Errors));
                }

                booking.Status = BookingStatus.Cancelled;
                _unitOfWork.Bookings.Update(booking);

                var trip = await _unitOfWork.Trips.GetByIdAsync(booking.TripId);
                if (trip != null)
                {
                    trip.AvailableSeats += booking.NumberOfTickets;
                    _unitOfWork.Trips.Update(trip);
                }

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PassengerTicketsResponseDto>> GetPassengerTicketsAsync(
            string passengerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var paged = await _unitOfWork.Tickets
                    .GetPaginatedTicketsByPassengerIdAsync(passengerId, pageNumber, pageSize);
                return ServiceResult<PassengerTicketsResponseDto>.Success(new PassengerTicketsResponseDto
                {
                    TotalCount = paged.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Tickets = paged.Tickets.Select(MapTicketToDto).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets for passenger {PassengerId}", passengerId);
                return ServiceResult<PassengerTicketsResponseDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────
        private BookingDto MapBookingToDto(Domain.Entities.Booking b)
        {
            // Fallback to "Station #ID" so the frontend always shows something
            // even if the navigation property failed to load
            var startName = b.StartStation?.Name?.Trim();
            var endName = b.EndStation?.Name?.Trim();

            return new BookingDto
            {
                Id = b.Id,
                PassengerId = b.PassengerId,
                PassengerName = b.Passenger?.AppUser?.FullName ?? string.Empty,
                TripId = b.TripId,
                StartStationId = b.StartStationId,
                StartStationName = string.IsNullOrEmpty(startName)
                    ? $"Station #{b.StartStationId}" : startName,
                StartCityId = b.StartStation?.CityId ?? 0,
                StartCityName = b.StartStation?.City?.Name?.Trim() ?? string.Empty,
                EndStationId = b.EndStationId,
                EndStationName = string.IsNullOrEmpty(endName)
                    ? $"Station #{b.EndStationId}" : endName,
                EndCityId = b.EndStation?.CityId ?? 0,
                EndCityName = b.EndStation?.City?.Name?.Trim() ?? string.Empty,
                BookingDate = b.BookingDate,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                IsPaid = b.IsPaid,
                NumberOfTickets = b.NumberOfTickets,
                PaymentIntentId = b.PaymentIntentId,
                Tickets = b.Tickets?.Select(MapTicketToDto).ToList() ?? new()
            };
        }

        private TicketDto MapTicketToDto(Ticket t) => new()
        {
            Id = t.Id,
            TripId = t.TripId,
            BookingId = t.BookingId,
            PassengerId = t.PassengerId,
            PassengerName = t.Passenger?.AppUser?.FullName ?? string.Empty,
            SeatNumber = t.SeatNumber,
            Price = t.Price,
            PurchaseDate = t.PurchaseDate,
            IsUsed = t.IsUsed,
            TicketCode = t.TicketCode
        };

        private string GenerateRandomTicketCode() => _random.Next(1000, 10000).ToString();
    }
}