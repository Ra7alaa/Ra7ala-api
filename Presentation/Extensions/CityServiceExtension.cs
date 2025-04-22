using Infrastructure.Extensions;

namespace Presentation.Extensions
{
    public static class CityServiceExtension
    {
        public static IServiceCollection AddCityServices(this IServiceCollection services)
        {
            // Call the extension method from Infrastructure layer
            Infrastructure.Extensions.CityServicesExtension.AddCityServices(services);
            
            return services;
        }
    }
}