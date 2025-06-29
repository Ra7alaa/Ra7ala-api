using Application.DTOs.Payment;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Errors;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a payment intent for a booking
        /// </summary>
        /// <param name="createDto">Payment creation data</param>
        /// <returns>Payment intent with client secret</returns>
        [HttpPost("create-payment-intent")]
        [Authorize(Roles = "Passenger")]
        public async Task<ActionResult<ApiResponse>> CreatePaymentIntent([FromBody] CreatePaymentIntentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid payment data", ModelState));
                }

                var result = await _paymentService.CreatePaymentIntentAsync(createDto);

                if (!result.IsSuccess)
                {
                    return BadRequest(new ApiResponse(400, string.Join(", ", result.Errors)));
                }

                return Ok(new ApiResponse(200, "Payment intent created successfully", result.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent for booking {BookingId}", createDto.BookingId);
                return StatusCode(500, new ApiResponse(500, "An error occurred while processing the payment"));
            }
        }

        /// <summary>
        /// Confirms a payment and processes the booking
        /// </summary>
        /// <param name="confirmationDto">Payment confirmation data</param>
        /// <returns>Payment result with ticket information</returns>
        [HttpPost("confirm-payment")]
        [Authorize(Roles = "Passenger")]
        public async Task<ActionResult<ApiResponse>> ConfirmPayment([FromBody] PaymentConfirmationDto confirmationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid confirmation data", ModelState));
                }

                var result = await _paymentService.ConfirmPaymentAsync(confirmationDto);

                if (!result.IsSuccess)
                {
                    return BadRequest(new ApiResponse(400, string.Join(", ", result.Errors)));
                }

                if (result.Data?.IsSuccess == true)
                {
                    return Ok(new ApiResponse(200, "Payment confirmed successfully", result.Data));
                }
                else
                {
                    return BadRequest(new ApiResponse(400, result.Data?.ErrorMessage ?? "Payment confirmation failed"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment {PaymentIntentId}", confirmationDto.PaymentIntentId);
                return StatusCode(500, new ApiResponse(500, "An error occurred while confirming the payment"));
            }
        }

        /// <summary>
        /// Gets payment status from Stripe
        /// </summary>
        /// <param name="paymentIntentId">Stripe Payment Intent ID</param>
        /// <returns>Payment status and details</returns>
        [HttpGet("status/{paymentIntentId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> GetPaymentStatus(string paymentIntentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentIntentId))
                {
                    return BadRequest(new ApiResponse(400, "Payment intent ID is required"));
                }

                var result = await _paymentService.GetPaymentStatusAsync(paymentIntentId);

                if (!result.IsSuccess)
                {
                    return BadRequest(new ApiResponse(400, string.Join(", ", result.Errors)));
                }

                return Ok(new ApiResponse(200, "Payment status retrieved successfully", result.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for {PaymentIntentId}", paymentIntentId);
                return StatusCode(500, new ApiResponse(500, "An error occurred while retrieving payment status"));
            }
        }

        /// <summary>
        /// Processes a refund for a payment
        /// </summary>
        /// <param name="refundDto">Refund data</param>
        /// <returns>Refund result</returns>
        [HttpPost("refund")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse>> ProcessRefund([FromBody] RefundDto refundDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid refund data", ModelState));
                }

                var result = await _paymentService.ProcessRefundAsync(refundDto);

                if (!result.IsSuccess)
                {
                    return BadRequest(new ApiResponse(400, string.Join(", ", result.Errors)));
                }

                return Ok(new ApiResponse(200, "Refund processed successfully", result.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment {PaymentIntentId}", refundDto.PaymentIntentId);
                return StatusCode(500, new ApiResponse(500, "An error occurred while processing the refund"));
            }
        }

        /// <summary>
        /// Handles Stripe webhook events
        /// </summary>
        /// <returns>Success or failure result</returns>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var signature = HttpContext.Request.Headers["Stripe-Signature"].FirstOrDefault();

                if (string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning("Webhook received without Stripe signature");
                    return BadRequest("Missing Stripe signature");
                }

                var result = await _paymentService.HandleWebhookAsync(payload, signature);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Webhook processing failed: {Errors}", string.Join(", ", result.Errors));
                    return BadRequest(string.Join(", ", result.Errors));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling webhook");
                return StatusCode(500, "Webhook processing failed");
            }
        }

        /// <summary>
        /// Gets Stripe publishable key for frontend
        /// </summary>
        /// <returns>Stripe publishable key</returns>
        [HttpGet("config")]
        [Authorize]
        public IActionResult GetStripeConfig()
        {
            try
            {
                // Only return non-sensitive configuration
                var config = new
                {
                    PublishableKey = "pk_test_51RKm5oIYSOR8cCsxAVTcbeYKIZHDQUWHBEPouuhqnbxdH3iTZxxoKbdHKQKGNhSLQ3svnidrJa9kEy37XQUzxBXk00JBAKL4f1",
                    Currency = "usd"
                };

                return Ok(new ApiResponse(200, "Stripe configuration retrieved successfully", config));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Stripe configuration");
                return StatusCode(500, new ApiResponse(500, "An error occurred while retrieving configuration"));
            }
        }
    }
}
