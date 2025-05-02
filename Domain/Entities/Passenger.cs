namespace Domain.Entities
{
    public class Passenger : BaseEntity<string>
    {
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>(); 
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); 
    }
}