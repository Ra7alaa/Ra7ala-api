namespace Infrastructure.ExternalServices.ChatBotService
{
    public class GeminiSettings
    {
        public required string ApiKey { get; set; }
        public string Model { get; set; } = "gemini-3.1-flash-lite-preview";
        public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent";
        public int MaxRetries { get; set; } = 3;
    }
}