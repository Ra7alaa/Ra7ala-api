using Application.DTOs.Auth;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<AppUser> userManager,
            IJwtService jwtService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return ServiceResult<AuthResponseDto>.Failure("Invalid username or password.");

            var token = await _jwtService.GenerateTokenAsync(user);
            
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                FullName = user.FullName,
                UserType = user.UserType.ToString()
            });
        }

        public async Task<ServiceResult<AuthResponseDto>> RegisterPassengerAsync(PassengerRegistrationDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return ServiceResult<AuthResponseDto>.Failure("Email is already taken.");

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return ServiceResult<AuthResponseDto>.Failure("Username is already taken.");

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                ProfilePictureUrl = model.ProfilePictureUrl,
                UserType = UserType.Passenger,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return ServiceResult<AuthResponseDto>.Failure(result.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRoleAsync(user, "Passenger");
            
            var token = await _jwtService.GenerateTokenAsync(user);

            return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                FullName = user.FullName,
                UserType = user.UserType.ToString()
            });
        }

        public async Task<ServiceResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult.Success();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                await _emailService.SendPasswordResetEmailAsync(email, user.UserName!, token);
                return ServiceResult.Success();
            }
            catch (Exception)
            {
                return ServiceResult.Failure("Failed to send password reset email. Please try again later.");
            }
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return ServiceResult.Failure("Password reset failed.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            
            return result.Succeeded 
                ? ServiceResult.Success() 
                : ServiceResult.Failure(result.Errors.Select(e => e.Description).ToList());
        }
    }
}