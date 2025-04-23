using Application.Services.City;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class CityServiceExtension
    {
        public static IServiceCollection AddCityServices(this IServiceCollection services)
        {
            services.AddScoped<ICityService, CityService>();
            
            return services;
        }
    }
}