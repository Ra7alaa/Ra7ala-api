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
        /// Processes a chat message from the user and returns a response from the AI chatbot.
        /// </summary>
        /// <param name="request">The chat request from the user.</param>
        /// <returns>A response from the AI chatbot.</returns>
        [HttpPost("message")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessChatMessage([FromBody] ChatBotRequestDto request)
        {
            var result = await _chatBotService.ProcessChatMessageAsync(request);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Chat message processed successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }
    }
}