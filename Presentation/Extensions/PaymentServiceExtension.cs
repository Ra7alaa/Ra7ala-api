using Application.Models;
using Application.Services.Interfaces;
using Infrastructure.ExternalServices.PaymentService;

namespace Presentation.Extensions
{
    public static class PaymentServiceExtension
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Stripe settings from appsettings.json
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

            // Register payment service (Stripe SDK manages its own HttpClient internally)
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
