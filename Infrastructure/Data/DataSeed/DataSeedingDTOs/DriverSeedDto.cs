using Domain.Enums;

namespace Infrastructure.Data.DataSeed
{
    internal class DriverSeedDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int CompanyId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public DriverStatus DriverStatus { get; set; }
    }
}