using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Noteapp.Infrastructure.Data;
using Noteapp.Infrastructure.Identity;
using System.Threading.Tasks;

namespace Noteapp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SeedDatabases(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static async Task SeedDatabases(IHost host)
        {
            using var scope = host.Services.CreateScope();

            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            identityContext.Database.EnsureDeleted();
            identityContext.Database.EnsureCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();
            await IdentityContextSeed.Seed(userManager);

            var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            appContext.Database.EnsureDeleted();
            appContext.Database.EnsureCreated();
            ApplicationContextSeed.Seed(appContext);
        }
    }
}
