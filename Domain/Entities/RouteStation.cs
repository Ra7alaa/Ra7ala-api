namespace Domain.Entities
{
    public class RouteStation
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual Station Station { get; set; } = null!;
    }
}

