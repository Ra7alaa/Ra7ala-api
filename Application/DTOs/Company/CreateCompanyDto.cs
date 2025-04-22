using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Company
{
    public class CreateCompanyDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public IFormFile? Logo { get; set; }
        
        // Super Admin details
        [Required]
        public string SuperAdminName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string SuperAdminEmail { get; set; } = string.Empty;
         
        [Required]
        [Phone]
        public string SuperAdminPhone { get; set; } = string.Empty;
    
    }
}