using Domain.Enums;

namespace Domain.Entities
{
    public class Booking : BaseEntity<int>
    {
        public string PassengerId { get; set; } = string.Empty;
        public int TripId { get; set; }
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public bool IsPaid { get; set; } = false;
        public int NumberOfTickets { get; set; } = 1;

        // Navigation properties
        public virtual Passenger Passenger { get; set; } = null!;
        public virtual Trip Trip { get; set; } = null!;
        public virtual Station StartStation { get; set; } = null!;
        public virtual Station EndStation { get; set; } = null!;
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}