using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.IsSuccess)
            {
                return Unauthorized(new { Errors = result.Errors });
            }
            
            return Ok(result.Data);
        }

        [HttpPost("register/passenger")]
        public async Task<IActionResult> RegisterPassenger([FromBody] PassengerRegistrationDto model)
        {
            var result = await _authService.RegisterPassengerAsync(model);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Errors = result.Errors });
            }
            
            return Ok(result.Data);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            await _authService.ForgotPasswordAsync(model.Email);
            
            // Always return OK to prevent email enumeration attacks
            return Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Errors = result.Errors });
            }
            
            return Ok(new { Message = "Password has been reset successfully" });
        }
    }
}