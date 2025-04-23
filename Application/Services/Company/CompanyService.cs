using Application.DTOs.Company;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IEmailService emailService,
            ILogger<CompanyService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto)
        {
            try
            {
                if (await _unitOfWork.CompanyRepository.ExistsAsync(createDto.Name, createDto.Email))
                    throw new InvalidOperationException("Company with this name or email already exists");

                var company = new Company
                {
                    Name = createDto.Name,
                    Email = createDto.Email,
                    Phone = createDto.Phone,
                    Address = createDto.Address,
                    Description = createDto.Description,
                    IsApproved = false,
                    IsRejected = false,
                    CreatedDate = DateTime.UtcNow
                };

                if (createDto.Logo != null)
                {
                    company.LogoUrl = await UploadLogoAsync(createDto.Logo);
                }

                await _unitOfWork.CompanyRepository.AddAsync(company);
                await _unitOfWork.SaveChangesAsync();

                return MapToCompanyDto(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                throw;
            }
        }

        public async Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber, int pageSize)
        {
            var (companies, totalCount) = await _unitOfWork.CompanyRepository
                .GetPagedCompaniesAsync(pageNumber, pageSize);

            return new CompanyListResponseDto
            {
                Companies = companies.Select(MapToCompanyDto).ToList(),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(id);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} not found");

            return MapToCompanyDto(company);
        }

        public async Task<List<CompanyDto>> GetPendingCompaniesAsync()
        {
            var companies = await _unitOfWork.CompanyRepository.GetPendingCompaniesAsync();
            return companies.Select(MapToCompanyDto).ToList();
        }

        public async Task<CompanyDto> UpdateCompanyAsync(int id, UpdateCompanyDto updateDto)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} not found");

            company.Name = updateDto.Name;
            company.Email = updateDto.Email;
            company.Phone = updateDto.Phone;
            company.Address = updateDto.Address;
            company.Description = updateDto.Description;

            if (updateDto.Logo != null)
            {
                company.LogoUrl = await UploadLogoAsync(updateDto.Logo);
            }

            _unitOfWork.CompanyRepository.Update(company);
            await _unitOfWork.SaveChangesAsync();

            return MapToCompanyDto(company);
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            if (company == null)
                return false;   

            company.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

      public async Task<CompanyDto> RateCompanyAsync(int companyId, int rating, string? comment, string userId)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var company = await _unitOfWork.CompanyRepository.GetCompanyWithRatingsAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            var companyFeedback = new CompanyFeedback
            {
                CompanyId = companyId,
                PassengerId = int.Parse(userId), // Fix string to int conversion
                Rating = rating,
                Comment = comment ?? string.Empty, // Fix null reference
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CompanyRepository.UpdateCompanyRatingAsync(companyId, rating);
            await _unitOfWork.SaveChangesAsync();

            return MapToCompanyDto(company);
        }
        
        public async Task<double> GetCompanyAverageRatingAsync(int companyId)
        {
            return await _unitOfWork.CompanyRepository.GetAverageRatingAsync(companyId);
        }

        public async Task<CompanyDto> ReviewCompanyRegistrationAsync(ReviewCompanyDto reviewDto)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(reviewDto.CompanyId);
            // if (company == null)
            //     throw new KeyNotFoundException($"Company with ID {reviewDto.CompanyId} not found");

            await _unitOfWork.CompanyRepository.UpdateCompanyStatusAsync(
                reviewDto.CompanyId,
                reviewDto.IsApproved,
                reviewDto.RejectionReason
            );

            if (reviewDto.IsApproved)
            {
                await CreateDefaultAdminAccountAsync(company);
            }

            await _unitOfWork.SaveChangesAsync();
            return MapToCompanyDto(company);
        }

        private CompanyDto MapToCompanyDto(Company company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                IsApproved = company.IsApproved,
                IsRejected = company.IsRejected,
                // AverageRating = company.AverageRating,
                // TotalRatings = company.TotalRatings,
                CreatedDate = company.CreatedDate,
                ApprovedDate = company.ApprovedDate
            };
        }

       private async Task<string> UploadLogoAsync(IFormFile logo)
{
    try
    {
        if (logo == null || logo.Length == 0)
            throw new ArgumentException("No file was uploaded");

        // Validate file type
        var allowedTypes = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(logo.FileName).ToLowerInvariant();
        if (!allowedTypes.Contains(fileExtension))
            throw new ArgumentException("Invalid file type. Only .jpg, .jpeg and .png files are allowed.");

        // Validate file size (e.g., max 2MB)
        if (logo.Length > 2 * 1024 * 1024)
            throw new ArgumentException("File size exceeds maximum limit of 2MB");

        // Create unique filename
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        // Define upload path (you might want to get this from configuration)
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Combine path and filename
        var filePath = Path.Combine(uploadPath, fileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await logo.CopyToAsync(fileStream);
        }

        // Return the relative URL
        return $"/uploads/logos/{fileName}";
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error uploading company logo");
        throw;
    }
}

       private async Task CreateDefaultAdminAccountAsync(Company company)
    {
        try
        {
            var password = GenerateSecurePassword();
            var admin = new AppUser
            {
                UserName = $"admin_{company.Name.ToLower().Replace(" ", "_")}",
                Email = company.Email,
                EmailConfirmed = true,
                PhoneNumber = company.Phone,
                UserType = UserType.Admin
            };

            var result = await _userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create admin account: {string.Join(", ", result.Errors)}");
            }

            await _userManager.AddToRoleAsync(admin, "Admin");
            
            // Send email with credentials
            await _emailService.SendAdminCredentialsEmailAsync(admin.Email, admin.UserName, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default admin account for company {CompanyId}", company.Id);
            throw;
        }
    }

        private string GenerateSecurePassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 12) + "Aa1!";
        }
    }
}