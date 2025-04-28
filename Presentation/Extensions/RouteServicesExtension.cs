using Application.Services.Interfaces;
using Application.Services.Route;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class RouteServicesExtension
    {
        public static IServiceCollection AddRouteServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IRouteRepository, RouteRepository>();
            
            // Register services
            services.AddScoped<IRouteService, RouteService>();
            
            return services;
        }
    }
}