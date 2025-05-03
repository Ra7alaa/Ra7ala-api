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
            services.Configure<GeminiSettings>(config.GetSection("Gemini"));
            
            // Register HttpClient with timeout for Gemini service
            services.AddHttpClient<IChatBotService, ChatBotService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            // Register the ChatBot service
            services.AddScoped<IChatBotService, ChatBotService>();
            
            return services;
        }
    }
}