using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.Company
{
    public class CompanyOwnerDetailsDto
    {
        // Company Basic Info
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string? TaxDocumentUrl { get; set; }
        
        // Company Status
        public string Status { get; set; } = CompanyStatus.Pending.ToString();
        public string? RejectionReason { get; set; }
        
        // Timestamps
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        
        // Rating Info
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }

        // Super Admin Info
        public SuperAdmindetailsDto SuperAdmin { get; set; } = null!;

        // Statistics & Additional Info
        public StatisticsDto Statistics { get; set; } = null!;
    }

    public class SuperAdmindetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
    }

    public class StatisticsDto
    {
        public int TotalAdmins { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalBuses { get; set; }
        public int TotalRoutes { get; set; }
        public int TotalFeedbacks { get; set; }
    }
}