using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.City;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class CityServicesExtension
    {
        public static IServiceCollection AddCityServices(this IServiceCollection services)
        {
            services.AddScoped<ICityService, CityService>();
            
            return services;
        }
    }
}
