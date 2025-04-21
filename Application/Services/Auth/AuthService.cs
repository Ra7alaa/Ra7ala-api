using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService //: IAuthService
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

        // public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        // {
        //     // Find user by email or username
        //     var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername) 
        //         ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername);

        //     if (user == null)
        //         throw new Exception("Invalid credentials");

        //     // Validate password
        //     if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        //         throw new Exception("Invalid credentials");

        //     // Generate JWT token
        //     var token = await _jwtService.GenerateTokenAsync(user);

        //     // Return response with token and user info
        //     return new AuthResponseDto
        //     {
        //         Token = token,
        //         Id = user.Id,
        //         Email = user.Email!,
        //         Username = user.UserName!,
        //         FullName = user.FullName,
        //         UserType = user.UserType.ToString()
        //     };
        // }

        // public async Task<AuthResponseDto> RegisterPassengerAsync(PassengerRegistrationDto model)
        // {
        //     // Check if email already exists
        //     if (await _userManager.FindByEmailAsync(model.Email) != null)
        //         throw new Exception("Email is already taken");

        //     // Check if username already exists
        //     if (await _userManager.FindByNameAsync(model.Username) != null)
        //         throw new Exception("Username is already taken");

        //     // Create AppUser with UserType = Passenger
        //     var user = new AppUser
        //     {
        //         UserName = model.Username,
        //         Email = model.Email,
        //         PhoneNumber = model.PhoneNumber,
        //         FullName = model.FullName,
        //         Address = model.Address,
        //         DateOfBirth = model.DateOfBirth,
        //         ProfilePictureUrl = model.ProfilePictureUrl,
        //         UserType = UserType.Passenger,
        //         DateCreated = DateTime.UtcNow
        //     };

        //     // Create user with password
        //     var result = await _userManager.CreateAsync(user, model.Password);
        //     if (!result.Succeeded)
        //     {
        //         var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //         throw new Exception($"Failed to create user: {errors}");
        //     }

        //     // Add to Passenger role if you're using roles
        //     await _userManager.AddToRoleAsync(user, "Passenger");

        //     // Create associated Passenger profile (1:1)
        //     var passenger = new Passenger
        //     {
        //         Id = user.Id,
        //         AppUser = user
        //     };

        //     // Generate JWT token
        //     var token = await _jwtService.GenerateTokenAsync(user);

        //     // Return response with token and user info
        //     return new AuthResponseDto
        //     {
        //         Token = token,
        //         Id = user.Id,
        //         Email = user.Email!,
        //         Username = user.UserName!,
        //         FullName = user.FullName,
        //         UserType = user.UserType.ToString()
        //     };
        // }

        // public async Task<bool> ForgotPasswordAsync(string email)
        // {
        //     var user = await _userManager.FindByEmailAsync(email);
        //     if (user == null)
        //     {
        //         // Don't reveal that the user does not exist
        //         return true;
        //     }

        //     // Generate password reset token
        //     var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //     // Send email with reset token
        //     await _emailService.SendPasswordResetEmailAsync(
        //         email, 
        //         user.UserName!, 
        //         token
        //     );

        //     return true;
        // }

        // public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        // {
        //     var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        //     if (user == null)
        //     {
        //         // Don't reveal that the user does not exist
        //         throw new Exception("Password reset failed");
        //     }

        //     var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        //     if (!result.Succeeded)
        //         throw new Exception("Password reset failed");

        //     return true;
        // }
    }
}