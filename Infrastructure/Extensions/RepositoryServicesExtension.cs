using Domain.Repositories.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class RepositoryServicesExtension
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Register the generic repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            // Register the Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
    }
}