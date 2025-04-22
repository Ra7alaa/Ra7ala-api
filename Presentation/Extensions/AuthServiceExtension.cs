using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class AuthServiceExtension 
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            // Register Auth Service
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}