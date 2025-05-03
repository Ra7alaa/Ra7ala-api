using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.ChatBot
{
    #region Request DTOs

    public class GeminiRequestDto
    {
        [JsonPropertyName("contents")]
        public List<GeminiContentDto> Contents { get; set; } = new List<GeminiContentDto>();
    }

    public class GeminiContentDto
    {
        [JsonPropertyName("parts")]
        public List<GeminiPartDto> Parts { get; set; } = new List<GeminiPartDto>();
    }

    public class GeminiPartDto
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    #endregion

    #region Response DTOs

    public class GeminiResponseDto
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidateDto> Candidates { get; set; }
        
        [JsonPropertyName("promptFeedback")]
        public GeminiPromptFeedbackDto PromptFeedback { get; set; }
    }

    public class GeminiCandidateDto
    {
        [JsonPropertyName("content")]
        public GeminiContentDto Content { get; set; }
        
        [JsonPropertyName("finishReason")]
        public string FinishReason { get; set; }
        
        [JsonPropertyName("index")]
        public int Index { get; set; }
        
        [JsonPropertyName("safetyRatings")]
        public List<GeminiSafetyRatingDto> SafetyRatings { get; set; }
    }

    public class GeminiSafetyRatingDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        
        [JsonPropertyName("probability")]
        public string Probability { get; set; }
    }

    public class GeminiPromptFeedbackDto
    {
        [JsonPropertyName("safetyRatings")]
        public List<GeminiSafetyRatingDto> SafetyRatings { get; set; }
    }

    #endregion
}