using Application.Models;
using Application.Services.Interfaces;
using Infrastructure.ExternalServices.PaymentService;

namespace Presentation.Extensions
{
    public static class PaymentServiceExtension
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Stripe settings
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            
            // Register HTTP client for Stripe (if needed for custom operations)
            services.AddHttpClient<IPaymentService, PaymentService>();
            
            // Register payment service
            services.AddScoped<IPaymentService, PaymentService>();
            
            return services;
        }
    }
}
