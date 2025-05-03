using Application.DTOs.ChatBot;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Errors;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        /// <summary>
        /// Process a user message and get a response from the AI chatbot.
        /// </summary>
        /// <param name="request">The chat request containing the user message</param>
        /// <returns>A response from the AI chatbot</returns>
        [HttpPost("message")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        public async Task<IActionResult> ProcessMessage([FromBody] ChatBotRequestDto request)
        {
            var result = await _chatBotService.ProcessMessageAsync(request);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Message processed successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }
    }
}