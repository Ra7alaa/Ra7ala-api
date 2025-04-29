using Domain.Enums;

namespace Domain.Entities
{
    public class Driver : BaseEntity<string>
    {
        public int CompanyId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string ContactAddress { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DriverStatus DriverStatus { get; set; } = DriverStatus.Active;
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public virtual ICollection<Trip> Trips { get; set; } = null!;
    }
}