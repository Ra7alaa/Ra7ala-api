using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class DriverDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public IFormFile? ProfilePicture { get; set; }
        
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime LicenseExpiryDate { get; set; }
        
        [Required]
        public string ContactAddress { get; set; } = string.Empty;
        
        [Required]
        public DateTime HireDate { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
    }
}