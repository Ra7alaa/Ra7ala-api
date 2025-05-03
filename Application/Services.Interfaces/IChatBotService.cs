using Application.DTOs.ChatBot;
using Application.Models;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IChatBotService
    {
        Task<ServiceResult<ChatBotResponseDto>> ProcessChatMessageAsync(ChatBotRequestDto request);
    }
}