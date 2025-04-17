using Domain.Enums;
namespace Domain.Entities
{
    public class Driver
    {
        public string Id { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string ContactAddress { get; set; } = string.Empty;
        //public string EmergencyContactName { get; set; } = string.Empty;
        //public string EmergencyContactNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DriverStatus DriverStatus { get; set; } = DriverStatus.Active;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
    }
}