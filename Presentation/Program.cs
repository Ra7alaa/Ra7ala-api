using Presentation.Extensions;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI using extension method
            builder.Services.AddSwaggerServices();
            
            // Configure CORS using extension method
            builder.Services.AddCorsServices(builder.Configuration);

            // Configure Email services using extension method
            builder.Services.AddEmailServices(builder.Configuration);
            
            var app = builder.Build();

            // Configure the Swagger middleware using extension method
            app.UseSwaggerMiddleware();
            
            // Use CORS middleware (should be before authentication)
            app.UseCorsMiddleware();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
