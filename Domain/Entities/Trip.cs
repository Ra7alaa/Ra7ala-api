namespace Domain.Entities
{
    public class Trip : BaseEntity<int>
    {
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; } 
        public string DriverId { get; set; } = string.Empty;
        public int? BusId { get; set; } 
        public bool IsCompleted { get; set; } = false;
        public int AvailableSeats { get; set; }
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual Driver Driver { get; set; } = null!;
        public virtual Bus? Bus { get; set; }
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<TripStation> TripStations { get; set; } = new List<TripStation>(); 
    }
}