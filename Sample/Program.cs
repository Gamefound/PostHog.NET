using PostHog.DI;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            builder.Services.AddControllers();

            services.AddPostHog("api-key", config =>
            {
                // leave empty if you are not self-hosting 
                config.Host = "example.com";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}