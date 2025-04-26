using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                return Unauthorized(new ApiResponse(401, "Invalid login credentials"));
            }

            return Ok(new ApiResponse(200, "Login successful") { Data = result.Data });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] string email)
        {
            var result = await _authService.ForgotPasswordAsync(email);
            
            return Ok(new ApiResponse(200, "If the email exists in our system, a password reset link has been sent"));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "Password has been reset successfully"));
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = await _authService.ChangePasswordAsync(User, changePasswordDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "Password has been changed successfully"));
        }

        [HttpPost("register-super-admin")]
        public async Task<ActionResult<ApiResponse>> RegisterSuperAdmin(SuperAdminDto superAdminDto)
        {
            var result = await _authService.RegisterSuperAdminAsync(superAdminDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(201, "Super Admin registered successfully"));
        }

        [HttpPost("register-passenger")]
        public async Task<ActionResult<ApiResponse>> RegisterPassenger(PassengerDto passengerDto)
        {
            var result = await _authService.RegisterPassengerAsync(passengerDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return StatusCode(201, new ApiResponse(201, "Passenger registered successfully"));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-driver")]
        public async Task<ActionResult<ApiResponse>> RegisterDriver(DriverDto driverDto)
        {
            var result = await _authService.RegisterDriverAsync(User, driverDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return StatusCode(201, new ApiResponse(201, "Driver registered successfully"));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-admin")]
        public async Task<ActionResult<ApiResponse>> RegisterAdmin(AdminDto adminDto)
        {
            var result = await _authService.RegisterAdminAsync(User, adminDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return StatusCode(201, new ApiResponse(201, "Admin registered successfully"));
        }
    }
}