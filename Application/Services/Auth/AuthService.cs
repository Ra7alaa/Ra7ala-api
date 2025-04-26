using Application.DTOs.Auth;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            IEmailService emailService,
            IFileService fileService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        #region Common Authentication Methods

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            // Try to find the user by email or username
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername);
            
            // If not found by email, try to find by username
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginDto.EmailOrUsername);
            }
            
            if (user == null)
            {
                return ServiceResult<AuthResponseDto>.Failure("Invalid login attempt.");
            }

            // Check the password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (!result.Succeeded)
            {
                return ServiceResult<AuthResponseDto>.Failure("Invalid login attempt.");
            }

            // Update last login time
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT token
            var token = await _jwtService.GenerateTokenAsync(user);
            
            // Create response with user information
            var authResponse = new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                UserType = user.UserType,
                Token = token
            };
            
            return ServiceResult<AuthResponseDto>.Success(authResponse);
        }

        public async Task<ServiceResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return ServiceResult.Success();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmailAsync(email, user.UserName ?? email, token);
            
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return ServiceResult.Failure("Password reset failed.");
            }

            var resetResult = await _userManager.ResetPasswordAsync(
                user, 
                resetPasswordDto.Token, 
                resetPasswordDto.NewPassword);

            if (!resetResult.Succeeded)
            {
                var errors = resetResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordDto changePasswordDto)
        {
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return ServiceResult.Failure("Authentication required.");
            }

            // Get the user ID from ClaimsPrincipal
            var userId = _userManager.GetUserId(user);
            
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult.Failure("User ID not found in authentication token.");
            }

            // Get the user by ID
            var appUser = await _userManager.FindByIdAsync(userId);
            
            if (appUser == null)
            {
                return ServiceResult.Failure("User not found.");
            }

            // Change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(
                appUser, 
                changePasswordDto.CurrentPassword, 
                changePasswordDto.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            // Send notification email
            var email = appUser.Email ?? string.Empty;
            var username = appUser.UserName ?? email;
            await _emailService.SendPasswordChangedNotificationAsync(
                email, 
                username);

            return ServiceResult.Success();
        }

        #endregion

        #region Registration Methods

        public async Task<ServiceResult<IdentityResult>> RegisterSuperAdminAsync(SuperAdminDto superAdminDto)
        {
            // Check if company already has a SuperAdmin
            var hasSuperAdmin = await _unitOfWork.Users.IsSuperAdminExistsForCompanyAsync(superAdminDto.CompanyId);
            
            if (hasSuperAdmin)
            {
                return ServiceResult<IdentityResult>.Failure("This company already has a Super Admin.");
            }

            // Check if user with email already exists
            var normalizedEmail = _userManager.NormalizeEmail(superAdminDto.Email);
            var existingUserByNormalizedEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            var existingUserByEmail = await _userManager.FindByEmailAsync(superAdminDto.Email);
            
            if (existingUserByEmail != null || existingUserByNormalizedEmail != null)
            {
                return ServiceResult<IdentityResult>.Failure($"Email '{superAdminDto.Email}' is already taken.");
            }

            // Create AppUser with just the basic required information
            var appUser = new AppUser
            {
                UserName = superAdminDto.Email,
                Email = superAdminDto.Email,
                PhoneNumber = superAdminDto.PhoneNumber,
                FullName = superAdminDto.FullName,
                UserType = UserType.SuperAdmin
            };

            // Generate a random password
            var password = GenerateRandomPassword();
            
            // Create user with generated password
            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                return ServiceResult<IdentityResult>.Failure(result.Errors.Select(e => e.Description));
            }

            // Assign role
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

            // Create SuperAdmin entity
            var superAdmin = new SuperAdmin
            {
                Id = appUser.Id,
                CompanyId = superAdminDto.CompanyId
            };

            // Add SuperAdmin to database
            _unitOfWork.Add(superAdmin);
            await _unitOfWork.SaveChangesAsync();

            // Get company name for the email
            var company = await _unitOfWork.Companies.GetByIdAsync(superAdminDto.CompanyId);
            string companyName = company?.Name ?? "Ra7ala Company";

            // Send email with credentials
            await _emailService.SendCompanyUserCredientialsEmailAsync(
                appUser.Email,
                appUser.Email,
                password,
                appUser.FullName,
                "Super Admin",
                companyName);

            return ServiceResult<IdentityResult>.Success(result);
        }

        public async Task<ServiceResult<IdentityResult>> RegisterPassengerAsync(PassengerDto passengerDto)
        {
            // Check if the profile picture was provided
            if (passengerDto.ProfilePicture == null || passengerDto.ProfilePicture.Length == 0)
            {
                return ServiceResult<IdentityResult>.Failure("Profile picture is required for passenger registration.");
            }

            // Check if user with email already exists
            var existingUser = await _userManager.FindByEmailAsync(passengerDto.Email);
            
            if (existingUser != null)
            {
                return ServiceResult<IdentityResult>.Failure("Email is already taken.");
            }

            // Save profile picture
            string profilePicPath = await _fileService.SaveFileAsync(passengerDto.ProfilePicture, "ProfilePictures");

            // Create AppUser
            var appUser = new AppUser
            {
                UserName = passengerDto.Email,
                Email = passengerDto.Email,
                PhoneNumber = passengerDto.PhoneNumber,
                FullName = passengerDto.FullName,
                ProfilePictureUrl = profilePicPath,
                DateOfBirth = passengerDto.DateOfBirth,
                Address = passengerDto.Address,
                UserType = UserType.Passenger
            };

            // Create user with password
            var result = await _userManager.CreateAsync(appUser, passengerDto.Password);

            if (!result.Succeeded)
            {
                // Delete uploaded image if user creation fails
                if (!string.IsNullOrEmpty(profilePicPath))
                {
                    _fileService.DeleteFile(profilePicPath);
                }
                return ServiceResult<IdentityResult>.Failure(result.Errors.Select(e => e.Description));
            }

            // Assign role
            await _userManager.AddToRoleAsync(appUser, "Passenger");

            // Create Passenger entity
            var passenger = new Passenger
            {
                Id = appUser.Id
            };

            // Add Passenger to database
            _unitOfWork.Add(passenger);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<IdentityResult>.Success(result);
        }

        public async Task<ServiceResult<IdentityResult>> RegisterDriverAsync(ClaimsPrincipal user, DriverDto driverDto)
        {
            // Authentication validation checks
            if (user == null)
            {
                return ServiceResult<IdentityResult>.Failure("Authentication required: user is null");
            }
            
            if (!user.Identity.IsAuthenticated)
            {
                return ServiceResult<IdentityResult>.Failure("Authentication required: user is not authenticated");
            }

            // Get the current super admin's user ID
            var superAdminUserId = _userManager.GetUserId(user);
            
            if (superAdminUserId == null)
            {
                return ServiceResult<IdentityResult>.Failure("User ID not found in authentication token");
            }

            // Get role claims to validate the user is actually a SuperAdmin
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("SuperAdmin"))
            {
                return ServiceResult<IdentityResult>.Failure($"User is not in SuperAdmin role. Found roles: {string.Join(", ", roles)}");
            }

            // Get the super admin to get the company ID
            var superAdmin = await _unitOfWork.Users.GetSuperAdminByUserIdAsync(superAdminUserId);
            
            if (superAdmin == null)
            {
                return ServiceResult<IdentityResult>.Failure("SuperAdmin record not found in database for authenticated user");
            }

            // Debug detailed check for existing email
            var normalizedEmail = _userManager.NormalizeEmail(driverDto.Email);
            var existingUserByNormalizedEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            var existingUserByEmail = await _userManager.FindByEmailAsync(driverDto.Email);
            
            if (existingUserByEmail != null)
            {
                return ServiceResult<IdentityResult>.Failure($"Email '{driverDto.Email}' is already taken (via FindByEmailAsync)");
            }
            
            if (existingUserByNormalizedEmail != null)
            {
                return ServiceResult<IdentityResult>.Failure($"Email '{driverDto.Email}' is already taken (via normalized email: '{normalizedEmail}')");
            }

            // Proceed with driver creation
            // Check if the profile picture was provided
            string? profilePicPath = null;
            if (driverDto.ProfilePicture != null && driverDto.ProfilePicture.Length > 0)
            {
                profilePicPath = await _fileService.SaveFileAsync(driverDto.ProfilePicture, "ProfilePictures");
            }

            // Create AppUser
            var appUser = new AppUser
            {
                UserName = driverDto.Email,
                Email = driverDto.Email,
                PhoneNumber = driverDto.PhoneNumber,
                FullName = driverDto.FullName,
                ProfilePictureUrl = profilePicPath,
                DateOfBirth = driverDto.DateOfBirth,
                Address = driverDto.ContactAddress,
                UserType = UserType.Driver
            };

            // Generate a random password
            var password = GenerateRandomPassword();
            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                // Delete uploaded image if user creation fails
                if (!string.IsNullOrEmpty(profilePicPath))
                {
                    _fileService.DeleteFile(profilePicPath);
                }
                return ServiceResult<IdentityResult>.Failure(result.Errors.Select(e => e.Description));
            }

            // Assign role
            await _userManager.AddToRoleAsync(appUser, "Driver");

            // Create Driver entity
            var driver = new Driver
            {
                Id = appUser.Id,
                CompanyId = superAdmin.CompanyId,
                LicenseNumber = driverDto.LicenseNumber,
                LicenseExpiryDate = driverDto.LicenseExpiryDate,
                ContactAddress = driverDto.ContactAddress,
                HireDate = driverDto.HireDate
            };

            // Add Driver to database
            _unitOfWork.Add(driver);
            await _unitOfWork.SaveChangesAsync();

            // Get company name for the email
            var company = superAdmin.Company;
            string companyName = company?.Name ?? "Ra7ala Company";

            // Send email with credentials and company info
            await _emailService.SendCompanyUserCredientialsEmailAsync(
                appUser.Email,
                appUser.Email,
                password,
                appUser.FullName,
                "Driver",
                companyName);

            return ServiceResult<IdentityResult>.Success(result);
        }

        public async Task<ServiceResult<IdentityResult>> RegisterAdminAsync(ClaimsPrincipal user, AdminDto adminDto)
        {
            // Authentication validation checks
            if (user == null)
            {
                return ServiceResult<IdentityResult>.Failure("Authentication required: user is null");
            }
            
            if (!user.Identity.IsAuthenticated)
            {
                return ServiceResult<IdentityResult>.Failure("Authentication required: user is not authenticated");
            }

            // Get the current super admin's user ID
            var superAdminUserId = _userManager.GetUserId(user);
            
            if (superAdminUserId == null)
            {
                return ServiceResult<IdentityResult>.Failure("User ID not found in authentication token");
            }

            // Get role claims to validate the user is actually a SuperAdmin
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("SuperAdmin"))
            {
                return ServiceResult<IdentityResult>.Failure($"User is not in SuperAdmin role. Found roles: {string.Join(", ", roles)}");
            }

            // Get the super admin to get the company ID
            var superAdmin = await _unitOfWork.Users.GetSuperAdminByUserIdAsync(superAdminUserId);
            
            if (superAdmin == null)
            {
                return ServiceResult<IdentityResult>.Failure("SuperAdmin record not found in database for authenticated user");
            }

            // Debug detailed check for existing email
            var normalizedEmail = _userManager.NormalizeEmail(adminDto.Email);
            var existingUserByNormalizedEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            var existingUserByEmail = await _userManager.FindByEmailAsync(adminDto.Email);
            
            if (existingUserByEmail != null)
            {
                return ServiceResult<IdentityResult>.Failure($"Email '{adminDto.Email}' is already taken (via FindByEmailAsync)");
            }
            
            if (existingUserByNormalizedEmail != null)
            {
                return ServiceResult<IdentityResult>.Failure($"Email '{adminDto.Email}' is already taken (via normalized email: '{normalizedEmail}')");
            }

            // Proceed with admin creation
            // Save profile picture if provided
            string? profilePicPath = null;
            if (adminDto.ProfilePicture != null)
            {
                profilePicPath = await _fileService.SaveFileAsync(adminDto.ProfilePicture, "ProfilePictures");
            }

            // Create AppUser
            var appUser = new AppUser
            {
                UserName = adminDto.Email,
                Email = adminDto.Email,
                PhoneNumber = adminDto.PhoneNumber,
                FullName = adminDto.FullName,
                ProfilePictureUrl = profilePicPath,
                DateOfBirth = adminDto.DateOfBirth,
                Address = adminDto.Address,
                UserType = UserType.Admin
            };

            // Generate a random password
            var password = GenerateRandomPassword();
            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                // Delete uploaded image if user creation fails
                if (!string.IsNullOrEmpty(profilePicPath))
                {
                    _fileService.DeleteFile(profilePicPath);
                }
                return ServiceResult<IdentityResult>.Failure(result.Errors.Select(e => e.Description));
            }

            // Assign role
            await _userManager.AddToRoleAsync(appUser, "Admin");

            // Create Admin entity
            var admin = new Admin
            {
                Id = appUser.Id,
                CompanyId = superAdmin.CompanyId,
                Department = adminDto.Department,
                AddedById = superAdminUserId
            };

            // Add Admin to database
            _unitOfWork.Add(admin);
            await _unitOfWork.SaveChangesAsync();

            // Get company name for the email
            var company = superAdmin.Company;
            string companyName = company?.Name ?? "Ra7ala Company";

            // Send email with credentials and company info
            await _emailService.SendCompanyUserCredientialsEmailAsync(
                appUser.Email,
                appUser.Email,
                password,
                appUser.FullName,
                "Admin",
                companyName);

            return ServiceResult<IdentityResult>.Success(result);
        }

        #endregion

        #region Helper Methods

        private string GenerateRandomPassword()
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specials = "!@#$%^&*()_-+=<>?";
            
            var random = new Random();
            var password = new StringBuilder();

            // Add at least one of each character type
            password.Append(lowerChars[random.Next(lowerChars.Length)]);
            password.Append(upperChars[random.Next(upperChars.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(specials[random.Next(specials.Length)]);

            // Add additional random characters to reach desired length (e.g., 12)
            var allChars = lowerChars + upperChars + numbers + specials;
            for (int i = 0; i < 8; i++) // 8 more characters for a total of 12
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password characters
            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }

        #endregion
    }
}