namespace Infrastructure.ExternalServices.ChatBotService
{
    public class GeminiSettings
    {
        public string ApiKey { get; set; }
        public string Model { get; set; } = "gemini-2.0-flash";
        public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        public int MaxRetries { get; set; } = 3;
    }
}