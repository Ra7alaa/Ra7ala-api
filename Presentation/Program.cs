using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Presentation.Extensions;
using System.IO;

namespace Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Serialize all enums as strings (e.g. "Confirmed" instead of 1)
                    // This prevents frontend .toLowerCase() crashes on numeric BookingStatus values
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

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

            // Serve the Angular build from wwwroot/browser
            var browserRootPath = Path.Combine(app.Environment.WebRootPath ?? string.Empty, "browser");
            if (Directory.Exists(browserRootPath))
            {
                var browserFileProvider = new PhysicalFileProvider(browserRootPath);

                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    FileProvider = browserFileProvider
                });

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = browserFileProvider
                });
            }

            // Keep the default wwwroot static file pipeline for uploads and other files
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            // Add authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Configure SPA fallback only when the built frontend exists on the server.
            var browserIndexPath = Path.Combine(browserRootPath, "index.html");
            if (File.Exists(browserIndexPath))
            {
                app.MapFallbackToFile("browser/index.html");
            }
            else
            {
                app.Logger.LogWarning("SPA fallback disabled because '{BrowserIndexPath}' was not found.", browserIndexPath);
            }

            await app.RunAsync();
        }
    }
}
