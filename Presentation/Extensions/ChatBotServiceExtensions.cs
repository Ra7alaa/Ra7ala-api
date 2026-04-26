using Application.Services.Interfaces;
using Infrastructure.ExternalServices.ChatBotService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Presentation.Extensions
{
    public static class ChatBotServiceExtensions
    {
        public static IServiceCollection AddChatBotServices(this IServiceCollection services, IConfiguration config)
        {
            // Register Gemini settings from configuration
            // ApiKey is loaded from environment variable: Gemini__ApiKey
            services.Configure<GeminiSettings>(config.GetSection("Gemini"));

            // Register HttpClient with timeout — also handles IChatBotService DI binding
            services.AddHttpClient<IChatBotService, ChatBotService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}