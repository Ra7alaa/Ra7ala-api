namespace Domain.Entities
{
    public class Trip : BaseEntity<int>
    {
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public string DriverId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public string CompanyId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual Driver Driver { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}