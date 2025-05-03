namespace Domain.Entities
{
    public class Ticket : BaseEntity<int>
    {
        public int TripId { get; set; }
        public int? BookingId { get; set; } 
        public string PassengerId { get; set; } = string.Empty;
        public int? SeatNumber { get; set; } 
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;
        public string TicketCode { get; set; } = string.Empty;

        // Navigation properties
        public virtual Trip Trip { get; set; } = null!;
        public virtual Booking? Booking { get; set; } 
        public virtual Passenger? Passenger { get; set; } = null!;
    }
}