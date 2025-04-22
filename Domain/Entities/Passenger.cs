namespace Domain.Entities
{
    public class Passenger
    {
        public string Id { get; set; } = string.Empty;
        
        // Navigation property
        public AppUser AppUser { get; set; } = null!;
    }
}