namespace Domain.Entities
{
    public class TripStation : BaseEntity<int>
    {
        public int TripId { get; set; }
        public int StationId { get; set; }
        public int SequenceNumber { get; set; } 
        public DateTime? ArrivalTime { get; set; } 
        public DateTime DepartureTime { get; set; } 
        public bool IsActive { get; set; } = true; 

        // Navigation properties
        public virtual Trip Trip { get; set; } = null!;
        public virtual Station Station { get; set; } = null!;
    }
}