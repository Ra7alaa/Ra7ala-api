using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Collections.Generic;
using System.Linq;
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("update-super-admin")]
        public async Task<ActionResult<ApiResponse>> UpdateSuperAdmin([FromQuery] string? superAdminId, [FromForm] UpdateSuperAdminDto updateSuperAdminDto)
        {
            var result = await _authService.UpdateSuperAdminAsync(User, superAdminId, updateSuperAdminDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "SuperAdmin updated successfully"));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("update-admin")]
        public async Task<ActionResult<ApiResponse>> UpdateAdmin([FromQuery] string? adminId, [FromForm] UpdateAdminDto updateAdminDto)
        {
            var result = await _authService.UpdateAdminAsync(User, adminId, updateAdminDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "Admin updated successfully"));
        }

        [Authorize(Roles = "Passenger")]
        [HttpPut("update-passenger")]
        public async Task<ActionResult<ApiResponse>> UpdatePassenger([FromForm] UpdatePassengerDto updatePassengerDto)
        {
            var result = await _authService.UpdatePassengerAsync(User, updatePassengerDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "Passenger updated successfully"));
        }

        [Authorize(Roles = "Driver,Admin,SuperAdmin")]
        [HttpPut("update-driver")]
        public async Task<ActionResult<ApiResponse>> UpdateDriver([FromQuery] string? driverId, [FromForm] UpdateDriverDto updateDriverDto)
        {
            var result = await _authService.UpdateDriverAsync(User, driverId, updateDriverDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiValidationErrorResponse 
                { 
                    Errors = result.Errors 
                });
            }

            return Ok(new ApiResponse(200, "Driver updated successfully"));
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpGet("super-admin/{id}")]
        public async Task<ActionResult<SuperAdminProfileDto>> GetSuperAdminById(string id)
        {
            var result = await _authService.GetSuperAdminByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault() ?? "SuperAdmin not found"));
            }
            
            return Ok(result.Data);
        }

        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult<AdminProfileDto>> GetAdminById(string id)
        {
            var result = await _authService.GetAdminByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault() ?? "Admin not found"));
            }
            
            return Ok(result.Data);
        }

        //[Authorize(Roles = "Passenger,Admin,SuperAdmin")]
        [HttpGet("passenger/{id}")]
        public async Task<ActionResult<PassengerProfileDto>> GetPassengerById(string id)
        {
            var result = await _authService.GetPassengerByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault() ?? "Passenger not found"));
            }
            
            return Ok(result.Data);
        }

        //[Authorize(Roles = "Driver,Admin,SuperAdmin")]
        [HttpGet("driver/{id}")]
        public async Task<ActionResult<DriverProfileDto>> GetDriverById(string id)
        {
            var result = await _authService.GetDriverByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault() ?? "Driver not found"));
            }
            
            return Ok(result.Data);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("system-owner")]
        public async Task<ActionResult<SystemOwnerProfileDto>> GetSystemOwner()
        {
            var result = await _authService.GetSystemOwnerAsync();
            
            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault() ?? "System Owner not found"));
            }
            
            return Ok(result.Data);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("admins/company")]
        public async Task<ActionResult<IEnumerable<AdminProfileDto>>> GetAllAdminsInMyCompany()
        {
            var result = await _authService.GetAllAdminsInMyCompanyAsync(User);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve admins"));
            }
            
            return Ok(result.Data);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("drivers/company")]
        public async Task<ActionResult<IEnumerable<DriverProfileDto>>> GetAllDriversInMyCompany()
        {
            var result = await _authService.GetAllDriversInMyCompanyAsync(User);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve drivers"));
            }
            
            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<object>> GetMyProfile()
        {
            var result = await _authService.GetMyProfileAsync(User);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve profile"));
            }
            
            return Ok(result.Data);
        }
    }
}