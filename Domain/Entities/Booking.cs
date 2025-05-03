using Domain.Enums;

namespace Domain.Entities
{
    public class Booking : BaseEntity<int>
    {
        public string PassengerId { get; set; } = string.Empty;
        public int TripId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation properties
        public virtual Passenger Passenger { get; set; } = null!;
        public virtual Trip Trip { get; set; } = null!;
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}