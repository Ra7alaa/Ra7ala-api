using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class PassengerRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public string? ProfilePictureUrl { get; set; }
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}