using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Auth
{
    public class UpdateDriverDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public IFormFile? ProfilePicture { get; set; }
        
        public string LicenseNumber { get; set; } = string.Empty;
        
        public DateTime? LicenseExpiryDate { get; set; }
        
        public string ContactAddress { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
    }
}