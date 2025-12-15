using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Progetto.Infrastructure;
using Progetto.Services;

namespace Progetto.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Crea il database e applica seed data
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TemplateDbContext>();
                
                // Crea il database se non esiste e applica le migrations
                context.Database.Migrate();
                
                // Popola i dati iniziali solo se il database è vuoto
                DataGenerator.InitializeUsers(context);
                DataGenerator.InitializeBuoniDigitali(context);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(kestrel =>
                    {
                        kestrel.AddServerHeader = false; // OWASP: Remove Kestrel response header 
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
