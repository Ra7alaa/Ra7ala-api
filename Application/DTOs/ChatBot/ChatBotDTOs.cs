using System;
using System.Collections.Generic;

namespace Application.DTOs.ChatBot
{
    public class ChatBotRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
    }

    public class ChatBotResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}