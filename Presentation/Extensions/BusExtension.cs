using Application.Services.Bus;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class BusExtension
    {
        public static IServiceCollection AddBusServices(this IServiceCollection services)
        {
            services.AddScoped<IBusService, BusService>();
            return services;
        }
    }
}
