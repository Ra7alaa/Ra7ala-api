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

            var app = builder.Build();

            // Configure the Swagger middleware using extension method
            app.UseSwaggerMiddleware();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
