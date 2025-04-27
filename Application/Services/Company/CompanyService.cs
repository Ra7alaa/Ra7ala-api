using Application.DTOs.Auth;
using Application.DTOs.Company;
using Application.Map;
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
        private readonly IAuthService _authService;

        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IEmailService emailService,
            IAuthService authService,
            ILogger<CompanyService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _authService = authService;
            _logger = logger;
        }

        // Creates a new company using the provided data.
        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto)
        {
            try
            {
                // Check if company exists by name or email with proper case-insensitive comparison
                var nameExists = await _unitOfWork.CompanyRepository.ExistsAsync(
                    c => c.Name.ToLower() == createDto.Name.ToLower() && c.Status !=CompanyStatus.Deleted.ToString());
                    
                if (nameExists)
                    throw new InvalidOperationException($"A company with the name '{createDto.Name}' already exists");

                var emailExists = await _unitOfWork.CompanyRepository.ExistsAsync(
                    c => c.Email.ToLower() == createDto.Email.ToLower() && c.Status != CompanyStatus.Deleted.ToString());
                    
                if (emailExists)
                    throw new InvalidOperationException($"A company with the email '{createDto.Email}' already exists");

                // Check if super admin email exists in the system  
                var userExists = await _userManager.FindByEmailAsync(createDto.SuperAdminEmail);
                if (userExists != null)
                    throw new InvalidOperationException($"This email '{createDto.SuperAdminEmail}' already exists in the system");

                var superAdminEmailExists = await _unitOfWork.CompanyRepository.ExistsAsync(
                    u => u.SuperAdminEmail.ToLower() == createDto.SuperAdminEmail.ToLower() && u.Status != CompanyStatus.Deleted.ToString()); 

                if (superAdminEmailExists)
                    throw new InvalidOperationException($"A user with the email '{createDto.SuperAdminEmail}' already exists");   

                var company = new Company
                {
                    Name = createDto.Name.Trim(),
                    Email = createDto.Email.Trim().ToLower(),
                    Phone = createDto.Phone,
                    Address = createDto.Address.Trim(),
                    Description = createDto.Description.Trim(),
                    Status = CompanyStatus.Pending.ToString(),
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
                Expression<Func<Company, bool>> predicate = c => c.Status != CompanyStatus.Deleted.ToString();
                
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
            var filter = new CompanyFilterDto {Status = CompanyStatus.Pending.ToString()};
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Update the GetApprovedCompaniesAsync method to support pagination
        public async Task<CompanyListResponseDto> GetApprovedCompaniesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new CompanyFilterDto { Status = CompanyStatus.Approved.ToString() };
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Update the GetRejectedCompaniesAsync method to support pagination
        public async Task<CompanyListResponseDto> GetRejectedCompaniesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new CompanyFilterDto { Status = CompanyStatus.Rejected.ToString() };
            return await GetCompaniesAsync(pageNumber, pageSize, filter);
        }

        // Retrieves a paginated list of companies with Filter.
        public async Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber, int pageSize, CompanyFilterDto? filter = null)
        {
            try 
            {
                // Start with default predicate
                Expression<Func<Company, bool>> predicate = c => c.Status != CompanyStatus.Deleted.ToString();

                // Apply status filter if provided
                if (!string.IsNullOrEmpty(filter?.Status))
                {
                    if (Enum.TryParse<CompanyStatus>(filter.Status, ignoreCase: true, out var status))
                    {
                        predicate = CombinePredicates(predicate, c => c.Status == status.ToString());
                    }
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(filter?.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.Trim().ToLower();
                    predicate = CombinePredicates(predicate, c =>
                        c.Name.ToLower().Contains(searchTerm) ||
                        c.Email.ToLower().Contains(searchTerm) ||
                        c.Phone.Contains(searchTerm) ||
                        c.Address.ToLower().Contains(searchTerm));
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies with filter");
                throw;
            }
        }

        // Retrieves a specific company by its ID for owner.
        public async Task<CompanyOwnerDetailsDto> GetCompanyOwnerProfileAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(id);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} not found");

            return company.ToCompanyOwnerDetailsDto();
        }

        // Retrieves a specific company by its ID for super admin.
        public async Task<CompanySuperAdminDetailsDto> GetCompanySuperAminProfileAsync(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(id);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} not found");

            return company.ToCompanySuperAdminDetailsDto();
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
            };
        }

        // Retrieves the admin profile for a given company and user.
        public async Task<CompanyAdminProfileDto> GetCompanyAdminProfileAsync(int companyId, string userId)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyWithDetailsAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found");

            return company.ToCompanyAdminDetailsDto();
          
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

            company.Status = CompanyStatus.Deleted.ToString();
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Reviews a company's registration based on the provided details.
        public async Task<CompanyDto> ReviewCompanyRegistrationAsync(ReviewCompanyDto reviewDto)
        {
            var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(reviewDto.CompanyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {reviewDto.CompanyId} not found");

            await _unitOfWork.CompanyRepository.UpdateCompanyStatusAsync(
                reviewDto.CompanyId,
                reviewDto.IsApproved,
                reviewDto.RejectionReason
            );

            if (reviewDto.IsApproved)
            {
               var superAdminDto = new SuperAdminDto
                {
                    FullName = company.SuperAdminName,
                    Email = company.SuperAdminEmail,
                    PhoneNumber = company.SuperAdminPhone,
                    CompanyId = company.Id
                };

                var registerResult = await _authService.RegisterSuperAdminAsync(superAdminDto);

                if (!registerResult.IsSuccess)
                {
                    throw new Exception($"Failed to create Super Admin after company approval: {string.Join(", ", registerResult.Errors)}");
                }
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
            try
            {
                return await _unitOfWork.CompanyRepository.GetAverageRatingAsync(companyId);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average rating for company {CompanyId}", companyId);
                throw;
            }
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
                Status= company.Status.ToString(),
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

       
    }
}