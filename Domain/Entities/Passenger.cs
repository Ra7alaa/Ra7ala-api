namespace Domain.Entities
{
    public class Passenger
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        // public string PreferredPaymentMethod { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public AppUser AppUser { get; set; } = null!;
    }
}