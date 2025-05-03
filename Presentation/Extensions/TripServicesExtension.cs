using Application.Services.Interfaces;
using Application.Services.Trip;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class TripServicesExtension
    {
        public static IServiceCollection AddTripServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<ITripRepository, TripRepository>();
            
            // Register services
            services.AddScoped<ITripService, TripService>();
            
            return services;
        }
    }
}