namespace Infrastructure.ExternalServices.ChatBotService
{
    public class OpenAISettings
    {
        public required string ApiKey { get; set; }
        public required string Model { get; set; }
        public required string Endpoint { get; set; }
        public int MaxRetries { get; set; }
    }
}