using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Ticket
{
    // DTO for ticket details in responses
    public class TicketDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int? BookingId { get; set; }
        public string PassengerId { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int? SeatNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsUsed { get; set; }
        public string TicketCode { get; set; } = string.Empty;
    }

    // DTO for ticket validation
    public class ValidateTicketDto
    {
        [Required(ErrorMessage = "Ticket code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Ticket code must be 4 characters")]
        public string TicketCode { get; set; } = string.Empty;
    }

    // DTO for passenger tickets response with pagination
    public class PassengerTicketsResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
    }
}