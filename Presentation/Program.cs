using Microsoft.OpenApi.Models;
using Presentation.Extensions;

namespace Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddRepositoryServices();

            builder.Services.AddCompanyServices();
            
            builder.Services.AddBusServices();

            // Configure Identity services (must come before JWT services)
            builder.Services.AddIdentityServices(builder.Configuration);

            // Configure Swagger/OpenAPI using extension method
            builder.Services.AddSwaggerServices();

            // Configure CORS using extension method
            builder.Services.AddCorsServices(builder.Configuration);

            // Configure Email services using extension method
            builder.Services.AddEmailServices(builder.Configuration);

            // Configure JWT services
            builder.Services.AddJwtServices(builder.Configuration);
          
            // Configure City services
            builder.Services.AddCityServices();

            // Configure File services (must be registered before Auth service)
            builder.Services.AddFileService();
            
            // Configure Auth services
            builder.Services.AddAuthService();
            
            // Configure Repository services
            builder.Services.AddRepositoryServices();
            
            // Configure Station services
            builder.Services.AddStationServices();

            // Configure Route services
            builder.Services.AddRouteServices();

            // Configure Trip services
            builder.Services.AddTripServices();
            
            // Configure Booking services
            builder.Services.AddBookingServices();
            
            // Configure ChatBot services
            builder.Services.AddChatBotServices(builder.Configuration);

            // Configure Payment services
            builder.Services.AddPaymentServices(builder.Configuration);

            // Configure Data Seed services
            builder.Services.AddDataSeedServices();

            var app = builder.Build();

            // Seed database data
            await app.SeedDatabaseAsync();

            // Configure the Swagger middleware using extension method
            app.UseSwaggerMiddleware();

            // Use CORS middleware (should be before authentication)
            app.UseCorsMiddleware();

            // Add static files middleware
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            // Add authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
