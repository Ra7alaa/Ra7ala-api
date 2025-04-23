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
            
            builder.Services.AddCompanyServices();

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
            
            // Configure Auth services
            //builder.Services.AddAuthServices();
            
            // Configure Data Seed services
            builder.Services.AddDataSeedServices();

            var app = builder.Build();

            // Seed database data
            await app.SeedDatabaseAsync();

            // Configure the Swagger middleware using extension method
            app.UseSwaggerMiddleware();
            
            // Use CORS middleware (should be before authentication)
            app.UseCorsMiddleware();

            app.UseHttpsRedirection();
            
            // Add authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
