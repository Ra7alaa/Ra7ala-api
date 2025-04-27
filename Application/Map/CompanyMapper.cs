using Application.DTOs.Company;
using Domain.Entities;
using Domain.Enums;

namespace Application.Map
{
    public static class CompanyMapper
    {
        public static CompanySuperAdminDetailsDto ToCompanySuperAdminDetailsDto(this Company company)
        {
            return new CompanySuperAdminDetailsDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                AverageRating = company.AverageRating ?? 0,
                TotalRatings = company.TotalRatings ?? 0,
                CreatedDate = company.CreatedDate,
                // ApprovedDate = company.ApprovedDate,
                // Status =company.Status.ToString(), 

                SuperAdmin = company.SuperAdmin != null ? new SuperAdminDetailsDto
                {
                    Id = company.SuperAdmin.Id,
                    FullName = company.SuperAdmin.AppUser.FullName,
                    Email = company.SuperAdmin.AppUser.Email ?? string.Empty,
                    PhoneNumber = company.SuperAdmin.AppUser.PhoneNumber ?? string.Empty
                } : null!,

                Admins = company.Admins.Select(a => new AdminDetailsDto
                {
                    Id = a.Id,
                    FullName = a.AppUser.FullName,
                    Email = a.AppUser.Email ?? string.Empty,
                    PhoneNumber = a.AppUser.PhoneNumber ?? string.Empty,
                    Department = a.Department
                }).ToList(),

                Drivers = company.Drivers.Select(d => new DriverDetailsDto
                {
                    Id = d.Id,
                    FullName = d.AppUser.FullName,
                    Email = d.AppUser.Email ?? string.Empty,
                    PhoneNumber = d.AppUser.PhoneNumber ?? string.Empty,
                    LicenseNumber = d.LicenseNumber,
                    LicenseExpiryDate = d.LicenseExpiryDate,
                    ContactAddress = d.ContactAddress
                }).ToList(),




                Feedbacks = company.Feedbacks.Select(f => new FeedbackDetailsDto
                {
                    Id = f.Id,
                    PassengerName = f.Passenger?.FullName ?? "Unknown",
                    PassengerEmail = f.Passenger?.Email ?? "Unknown",
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                }).ToList()
            };
        }

 public static CompanyAdminProfileDto ToCompanyAdminDetailsDto(this Company company)
        {
            return new CompanyAdminProfileDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                AverageRating = company.AverageRating ?? 0,
                TotalRatings = company.TotalRatings ?? 0,

                Drivers = company.Drivers.Select(d => new DriverDetailsDto
                {
                    Id = d.Id,
                    FullName = d.AppUser.FullName,
                    Email = d.AppUser.Email ?? string.Empty,
                    PhoneNumber = d.AppUser.PhoneNumber ?? string.Empty,
                    LicenseNumber = d.LicenseNumber,
                    LicenseExpiryDate = d.LicenseExpiryDate,
                    ContactAddress = d.ContactAddress
                }).ToList(),




                Feedbacks = company.Feedbacks.Select(f => new FeedbackDetailsDto
                {
                    Id = f.Id,
                    PassengerName = f.Passenger?.FullName ?? "Unknown",
                    PassengerEmail = f.Passenger?.Email ?? "Unknown",
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                }).ToList()
            };
        }
        public static CompanyOwnerDetailsDto ToCompanyOwnerDetailsDto(this Company company)
        {
            return new CompanyOwnerDetailsDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                Status = Enum.Parse<CompanyStatus>(company.Status),
                RejectionReason = company.RejectionReason,
                CreatedDate = company.CreatedDate,
                ApprovedDate = company.ApprovedDate,
                AverageRating = company.AverageRating ?? 0,
                TotalRatings = company.TotalRatings ?? 0,

                SuperAdmin = company.SuperAdmin != null ? new SuperAdmindetailsDto
                {
                    Id = company.SuperAdmin.Id,
                    FullName = company.SuperAdmin.AppUser.FullName,
                    Email = company.SuperAdmin.AppUser.Email ?? string.Empty,
                    Phone = company.SuperAdmin.AppUser.PhoneNumber ?? string.Empty
                } : null!,

                Statistics = new StatisticsDto
                {
                    TotalAdmins = company.Admins?.Count ?? 0,
                    TotalDrivers = company.Drivers?.Count ?? 0,
                    TotalBuses = company.Buses?.Count ?? 0,
                    TotalRoutes = company.Routes?.Count ?? 0,
                    TotalFeedbacks = company.Feedbacks?.Count ?? 0
                }
            };
        }
    }
}
   