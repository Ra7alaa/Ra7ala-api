using Application.DTOs.Auth;
using Application.Models; 
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ServiceResult<AuthResponseDto>> RegisterPassengerAsync(PassengerRegistrationDto model);
        Task<ServiceResult> ForgotPasswordAsync(string email); 
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto); 
    }
}