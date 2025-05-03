using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.ChatBot
{
    public class ChatBotRequestDto
    {
        public required string UserMessage { get; set; }
        public required string UserId { get; set; }
        public string SessionId { get; set; } = System.Guid.NewGuid().ToString();
    }

    public class ChatBotResponseDto
    {
        public required string Response { get; set; }
        public required string SessionId { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ChatMessage
    {
        public required string Role { get; set; }
        public required string Content { get; set; }
    }

    public class ChatBotOpenAIRequestDto
    {
        public required string Model { get; set; }
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public double Temperature { get; set; } = 0.7;
    }

    public class ChatBotOpenAIResponseDto
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        
        [JsonPropertyName("object")]
        public required string Object { get; set; }
        
        [JsonPropertyName("created")]
        public long Created { get; set; }
        
        [JsonPropertyName("model")]
        public required string Model { get; set; }
        
        [JsonPropertyName("choices")]
        public required List<ChatBotOpenAIChoiceDto> Choices { get; set; }
        
        [JsonPropertyName("usage")]
        public required ChatBotOpenAIUsageDto Usage { get; set; }
    }

    public class ChatBotOpenAIChoiceDto
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        
        [JsonPropertyName("message")]
        public required ChatBotOpenAIMessageDto Message { get; set; }
        
        [JsonPropertyName("finish_reason")]
        public required string FinishReason { get; set; }
    }

    public class ChatBotOpenAIMessageDto
    {
        [JsonPropertyName("role")]
        public required string Role { get; set; }
        
        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }

    public class ChatBotOpenAIUsageDto
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}