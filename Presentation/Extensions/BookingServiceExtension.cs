using Application.Services.Booking;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Extensions
{
    public static class BookingServiceExtension
    {
        public static IServiceCollection AddBookingServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            
            // Register services
            services.AddScoped<IBookingService, BookingService>();
            
            return services;
        }
    }
}