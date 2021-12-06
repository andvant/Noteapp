using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Noteapp.Infrastructure.Data;
using Noteapp.Infrastructure.Identity;
using System;
using System.Threading.Tasks;

namespace Noteapp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SeedDatabases(host.Services);
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

        private static async Task SeedDatabases(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();
            var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            //identityContext.Database.EnsureDeleted();
            //appContext.Database.EnsureDeleted();
            identityContext.Database.EnsureCreated();
            appContext.Database.EnsureCreated();

            await IdentityContextSeed.Seed(userManager);
            ApplicationContextSeed.Seed(appContext);
        }
    }
}
