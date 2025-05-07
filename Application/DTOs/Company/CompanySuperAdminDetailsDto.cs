using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.Company
{
    public class CompanySuperAdminDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string? TaxDocumentUrl { get; set; } 
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public DateTime CreatedDate { get; set; }
        // public DateTime? ApprovedDate { get; set; }
        // public string Status { get; set; } = CompanyStatus.Pending.ToString();

        // Super Admin Details
        public SuperAdminDetailsDto SuperAdmin { get; set; } = null!;
        
        // Related Collections
        public ICollection<AdminDetailsDto> Admins { get; set; } = new List<AdminDetailsDto>();
        public ICollection<DriverDetailsDto> Drivers { get; set; } = new List<DriverDetailsDto>();
        public ICollection<BusDetailsDto> Buses { get; set; } = new List<BusDetailsDto>();
        public ICollection<RouteDetailsDto> Routes { get; set; } = new List<RouteDetailsDto>();
        public ICollection<FeedbackDetailsDto> Feedbacks { get; set; } = new List<FeedbackDetailsDto>();
    }

    public class SuperAdminDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AdminDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class DriverDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string ContactAddress { get; set; } = string.Empty;
    }

    public class BusDetailsDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
    }

    public class RouteDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Distance { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
    }

    public class FeedbackDetailsDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}