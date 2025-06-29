using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Application.DTOs.Ticket;

namespace Application.DTOs.Booking
{
    // DTO for creating a new booking
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Trip ID is required")]
        public int TripId { get; set; }

        [Required(ErrorMessage = "Start station ID is required")]
        public int StartStationId { get; set; }

        [Required(ErrorMessage = "End station ID is required")]
        public int EndStationId { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(1, 10, ErrorMessage = "Number of tickets must be between 1 and 10")]
        public int NumberOfTickets { get; set; } = 1;
    }

    // DTO for booking details response
    public class BookingDto
    {
        public int Id { get; set; }
        public string PassengerId { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int TripId { get; set; }
        public int StartStationId { get; set; }
        public string StartStationName { get; set; } = string.Empty;
        public int StartCityId { get; set; }
        public string StartCityName { get; set; } = string.Empty;
        public int EndStationId { get; set; }
        public string EndStationName { get; set; } = string.Empty;
        public int EndCityId { get; set; }
        public string EndCityName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public bool IsPaid { get; set; }
        public int NumberOfTickets { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
    }
    
    // DTO for passenger bookings
    public class PassengerBookingsResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();
    }
    
    // DTO for processing payment
    public class ProcessPaymentDto
    {
        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = "CreditCard";
        
        // Stripe payment method token (optional - if provided, payment will be processed immediately)
        public string? PaymentMethodToken { get; set; }
        
        // Additional customer information for Stripe
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
    }
    
    // DTO for confirming booking after payment
    public class BookingConfirmationDto
    {
        public int BookingId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
    }
}