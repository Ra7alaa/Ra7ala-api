using Application.DTOs.Payment;
using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a payment intent for a booking
        /// </summary>
        /// <param name="createDto">Payment creation data</param>
        /// <returns>Payment intent with client secret</returns>
        Task<ServiceResult<PaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentDto createDto);

        /// <summary>
        /// Confirms a payment and processes the booking
        /// </summary>
        /// <param name="confirmationDto">Payment confirmation data</param>
        /// <returns>Payment result with ticket information</returns>
        Task<ServiceResult<PaymentResultDto>> ConfirmPaymentAsync(PaymentConfirmationDto confirmationDto);

        /// <summary>
        /// Retrieves payment status from Stripe
        /// </summary>
        /// <param name="paymentIntentId">Stripe Payment Intent ID</param>
        /// <returns>Payment status and details</returns>
        Task<ServiceResult<PaymentResultDto>> GetPaymentStatusAsync(string paymentIntentId);

        /// <summary>
        /// Processes a refund for a payment
        /// </summary>
        /// <param name="refundDto">Refund data</param>
        /// <returns>Refund result</returns>
        Task<ServiceResult<RefundResultDto>> ProcessRefundAsync(RefundDto refundDto);

        /// <summary>
        /// Handles Stripe webhook events
        /// </summary>
        /// <param name="payload">Webhook payload</param>
        /// <param name="signature">Stripe signature header</param>
        /// <returns>Success or failure result</returns>
        Task<ServiceResult> HandleWebhookAsync(string payload, string signature);

        /// <summary>
        /// Updates booking status after successful payment
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <param name="paymentIntentId">Stripe Payment Intent ID</param>
        /// <returns>Success or failure result</returns>
        Task<ServiceResult> UpdateBookingAfterPaymentAsync(int bookingId, string paymentIntentId);

        /// <summary>
        /// Generates tickets after successful payment
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <returns>List of generated ticket IDs</returns>
        Task<ServiceResult<List<int>>> GenerateTicketsAsync(int bookingId);
    }
}
