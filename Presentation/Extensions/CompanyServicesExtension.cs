using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Company;
using Microsoft.Extensions.DependencyInjection;
namespace Presentation.Extensions
{
    public static class CompanyServicesExtension
    {
        public static IServiceCollection AddCompanyServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository<Domain.Entities.Company>, GenericRepository<Domain.Entities.Company>>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            
            // Register services
            services.AddScoped<ICompanyService, CompanyService>();
            
            return services;
        }
    }
}