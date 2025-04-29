namespace Domain.Entities
{
    public class RouteStation : BaseEntity<int>
    {
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual Station Station { get; set; } = null!;
    }
}

