using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
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

        public string LogoUrl { get; set; } = string.Empty;

        // Super Admin Data
        public string SuperAdminName { get; set; } = string.Empty;
        public string SuperAdminEmail { get; set; } = string.Empty;
        public string SuperAdminPhone { get; set; } = string.Empty;
        
        // Registration status
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        // Timestamps
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }

        // Rating and Feedback
        public double? AverageRating { get; set; }
        public int? TotalRatings { get; set; }
  
        // Navigation properties
        public SuperAdmin SuperAdmin { get; set; } = null!;
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();
        // public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
        public virtual ICollection<CompanyFeedback> Feedbacks { get; set; } = new List<CompanyFeedback>();
        public string? RejectionReason { get; set; }
    }
}