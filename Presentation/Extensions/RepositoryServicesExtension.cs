using Infrastructure.Extensions;

namespace Presentation.Extensions
{
    public static class RepositoryServicesExtension
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Call the extension method from Infrastructure layer
            Infrastructure.Extensions.RepositoryServicesExtension.AddRepositoryServices(services);
            
            return services;
        }
    }
}