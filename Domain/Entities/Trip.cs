namespace Domain.Entities
{
    public class Trip
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public string DriverId { get; set; }

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual Driver Driver { get; set; } = null!;
    }
}