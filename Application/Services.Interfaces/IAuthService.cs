using Application.DTOs.Auth;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        // Common methods
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ServiceResult> ForgotPasswordAsync(string email);
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ServiceResult> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordDto changePasswordDto);

        // Registration methods
        Task<ServiceResult<IdentityResult>> RegisterSuperAdminAsync(SuperAdminDto superAdminDto);
        Task<ServiceResult<IdentityResult>> RegisterPassengerAsync(PassengerDto passengerDto);
        Task<ServiceResult<IdentityResult>> RegisterDriverAsync(ClaimsPrincipal user, DriverDto driverDto);
        Task<ServiceResult<IdentityResult>> RegisterAdminAsync(ClaimsPrincipal user, AdminDto adminDto);
        
        // Update methods
        Task<ServiceResult> UpdateSuperAdminAsync(ClaimsPrincipal user, string? superAdminId, UpdateSuperAdminDto updateSuperAdminDto);
        Task<ServiceResult> UpdateAdminAsync(ClaimsPrincipal user, string? adminId, UpdateAdminDto updateAdminDto);
        Task<ServiceResult> UpdatePassengerAsync(ClaimsPrincipal user, UpdatePassengerDto updatePassengerDto);
        Task<ServiceResult> UpdateDriverAsync(ClaimsPrincipal user, string? driverId, UpdateDriverDto updateDriverDto);
    }
}