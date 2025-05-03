using Domain.Enums;

namespace Application.DTOs.Auth
{
    public class DriverProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DriverStatus driverStatus { get; set; }
        public string UserType { get; set; } = "Driver";
    }
}