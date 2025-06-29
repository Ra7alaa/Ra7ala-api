using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payment
{
    public class CreatePaymentIntentDto
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public string Currency { get; set; } = "usd";
        
        public string? Description { get; set; }
        
        // Optional: Customer information for Stripe
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int BookingId { get; set; }
    }

    public class PaymentConfirmationDto
    {
        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;
        
        [Required]
        public int BookingId { get; set; }
    }

    public class PaymentResultDto
    {
        public bool IsSuccess { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? ErrorMessage { get; set; }
        public int BookingId { get; set; }
        public List<int> TicketIds { get; set; } = new List<int>();
    }

    public class RefundDto
    {
        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;
        
        public decimal? Amount { get; set; } // If null, refund full amount
        
        public string? Reason { get; set; }
    }

    public class RefundResultDto
    {
        public bool IsSuccess { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class WebhookEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
