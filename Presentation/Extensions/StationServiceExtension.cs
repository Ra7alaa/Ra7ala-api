// filepath: e:\iti files\Grud project\Ra7ala-api\Presentation\Extensions\StationServiceExtension.cs
using Application.Services.Interfaces;
using Application.Services.Station;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class StationServiceExtension
    {
        public static IServiceCollection AddStationServices(this IServiceCollection services)
        {
            services.AddScoped<IStationService, StationService>();
            
            return services;
        }
    }
}