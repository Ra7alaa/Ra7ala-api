using Application.DTOs.Company;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

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

        // Creates a new company using the provided data.
        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto)
        {
            try
            {
                // Check if company exists by name or email with proper case-insensitive comparison
                var nameExists = await _unitOfWork.CompanyRepository.ExistsAsync(
                    c => c.Name.ToLower() == createDto.Name.ToLower() && !c.IsDeleted);
                    
                if (nameExists)
                    throw new InvalidOperationException($"A company with the name '{createDto.Name}' already exists");

                var emailExists = await _unitOfWork.CompanyRepository.ExistsAsync(
                    c => c.Email.ToLower() == createDto.Email.ToLower() && !c.IsDeleted);
                    
                if (emailExists)
                    throw new InvalidOperationException($"A company with the email '{createDto.Email}' already exists");

                var company = new Company
                {
                    Name = createDto.Name.Trim(),
                    Email = createDto.Email.Trim().ToLower(),
                    Phone = createDto.Phone,
                    Address = createDto.Address.Trim(),
                    Description = createDto.Description.Trim(),
                    IsApproved = false,
                    IsRejected = false,
                    RejectionReason = string.Empty,
                    SuperAdminName = createDto.SuperAdminName.Trim(),
                    SuperAdminEmail = createDto.SuperAdminEmail.Trim().ToLower(),
                    SuperAdminPhone = createDto.SuperAdminPhone,
                    CreatedDate = DateTime.UtcNow,
                    AverageRating = 0,
                    TotalRatings = 0
                };

                if (createDto.Logo != null)
                {
                    company.LogoUrl = await UploadLogoAsync(createDto.Logo);
                }

                await _unitOfWork.CompanyRepository.AddAsync(company);
                await _unitOfWork.SaveChangesAsync();

                return MapToCompanyDto(company);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company with name {Name} and email {Email}", 
                    createDto.Name, createDto.Email);
                throw;
            }
        }

        // Retrieves a paginated list of companies.
        public async Task<CompanyListResponseDto> GetAllCompaniesAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Create a default filter for non-deleted companies
                Expression<Func<Company, bool>> predicate = c => !c.IsDeleted;
                
                var (companies, totalCount) = await _unitOfWork.CompanyRepository
                    .GetPagedCompaniesAsync(pageNumber, pageSize, predicate);

                return new CompanyListResponseDto
                {
                    Companies = companies.Select(MapToCompanyDto).ToList(),
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all companies");
                throw;
            }
        }

        // Fix for the GetPendingCompaniesAsync method
        public async Task<CompanyListResponseDto> GetPendingCompaniesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new CompanyFilterDto { IsApproved = false, IsRejected = false, IsDeleted = false };
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Update the GetApprovedCompaniesAsync method to support pagination
        public async Task<CompanyListResponseDto> GetApprovedCompaniesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new CompanyFilterDto { IsApproved = true };
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Update the GetRejectedCompaniesAsync method to support pagination
        public async Task<CompanyListResponseDto> GetRejectedCompaniesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new CompanyFilterDto { IsRejected = true };
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Retrieves a paginated list of companies with Filter.
        public async Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber, int pageSize, CompanyFilterDto? filter = null)
        {
            // Start with not deleted as default predicate
            Expression<Func<Company, bool>> predicate = c => !c.IsDeleted;

            if (filter != null)
            {
                if (filter.IsApproved.HasValue)
                    predicate = CombinePredicates(predicate, c => c.IsApproved == filter.IsApproved.Value);

                if (filter.IsRejected.HasValue)
                    predicate = CombinePredicates(predicate, c => c.IsRejected == filter.IsRejected.Value);

                // Pending companies are those that are neither approved nor rejected
                if (!filter.IsApproved.HasValue && !filter.IsRejected.HasValue)
                    predicate = CombinePredicates(predicate, c => !c.IsApproved && !c.IsRejected);

                // Override IsDeleted filter only if specifically requested
                if (filter.IsDeleted.HasValue)
                    predicate = CombinePredicates(predicate, c => c.IsDeleted == filter.IsDeleted.Value);

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.Trim().ToLower();
                    predicate = CombinePredicates(predicate, c =>
                        c.Name.ToLower().Contains(searchTerm) ||
                        c.Email.ToLower().Contains(searchTerm) ||
                        c.Phone.Contains(searchTerm) ||
                        c.Address.ToLower().Contains(searchTerm));
                }
            }

            var (companies, totalCount) = await _unitOfWork.CompanyRepository
                .GetPagedCompaniesAsync(pageNumber, pageSize, predicate);

            return new CompanyListResponseDto
            {
                Companies = companies.Select(MapToCompanyDto).ToList(),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        // Retrieves a specific company by its ID.
        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(id);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} not found");

            return MapToCompanyDto(company);
        }

        public async Task<CompanyUserProfileDto> GetCompanyUserProfileAsync(int companyId)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(companyId);
            var averageRating = await GetCompanyAverageRatingAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            return new CompanyUserProfileDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                AverageRating = averageRating
                // TotalRatings = company.TotalRatings ?? 0
            };
        }

        // Retrieves the admin profile for a given company and user.
        public async Task<CompanyAdminProfileDto> GetCompanyAdminProfileAsync(int companyId, string userId)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            // Verify if user has access to this company
            // var userHasAccess = await _unitOfWork.CompanyRepository.UserHasAccessToCompany(userId, companyId);
            // if (!userHasAccess)
            //     throw new UnauthorizedAccessException("You don't have access to this company's admin profile");

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
                SuperAdminName = company.SuperAdminName,
                SuperAdminEmail = company.SuperAdminEmail,
                SuperAdminPhone = company.SuperAdminPhone,
                CreatedDate = company.CreatedDate,
                ApprovedDate = company.ApprovedDate,
                Admins = company.Admins.Select(a => new AdminInfoDto
                {
                    Name = a.AppUser.FullName,
                    Email = a.AppUser.Email ?? string.Empty,
                    Department = a.Department
                }).ToList(),
            };
        }
       
        // Updates an existing company with the provided data.
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

        // Deletes a company by its ID.
        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            if (company == null)
                return false;

            company.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Reviews a company's registration based on the provided details.
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

        // Allows users to rate a company and add a comment.
        public async Task RateCompanyAsync(int companyId, int rating, string? comment, string userId)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var company = await _unitOfWork.CompanyRepository.GetCompanyWithRatingsAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            var companyFeedback = new CompanyFeedback
            {
                CompanyId = companyId,
                PassengerId = userId, // Keep as string since it's the AppUser Id
                Rating = rating,
                Comment = comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            company.Feedbacks.Add(companyFeedback);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CompanyRepository.UpdateCompanyRatingAsync(companyId);

        }

        // Calculates the average rating for a given company.
        public async Task<int> GetCompanyAverageRatingAsync(int companyId)
        {
            return await _unitOfWork.CompanyRepository.GetAverageRatingAsync(companyId);
        }

        // Retrieves detailed ratings for a given company.
        public async Task<CompanyRatingsDto> GetCompanyRatingsDetailsAsync(int companyId)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithRatingsAsync(companyId);

            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            return new CompanyRatingsDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                AverageRating = company.AverageRating ?? 0,
                TotalRatings = company.TotalRatings ?? 0,
                Feedbacks = company.Feedbacks.Select(f => new FeedbackDto
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

        //Private Methods
        private Expression<Func<T, bool>> CombinePredicates<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(first, parameter),
                    Expression.Invoke(second, parameter)
                ), parameter);
            return combined;
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
                IsDeleted = company.IsDeleted,
                AverageRating = (double)(company.AverageRating ?? 0),
                TotalRatings = (int)(company.TotalRatings ?? 0),
                CreatedDate = company.CreatedDate,
                ApprovedDate = company.ApprovedDate?? DateTime.MinValue
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
                var superAdmin = new AppUser
                {
                    UserName = $"Superadmin_{company.SuperAdminName.ToLower().Replace(" ", "_")}",
                    Email = company.SuperAdminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = company.SuperAdminPhone,
                    UserType = UserType.Admin
                };

                var result = await _userManager.CreateAsync(superAdmin, password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create admin account: {string.Join(", ", result.Errors)}");
                }

                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                // Send email with credentials
                await _emailService.SendAdminCredentialsEmailAsync(company.Name, superAdmin.Email, superAdmin.UserName, password);
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