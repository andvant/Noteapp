using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Noteapp.Core.Interfaces;
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

            await SeedRepositories(host);

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

        private static async Task SeedRepositories(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();
                await IdentityContextSeed.Seed(userManager);

                var userRepository = scope.ServiceProvider.GetRequiredService<IAppUserRepository>();
                AppUserRepositorySeed.Seed(userRepository);

                var noteRepository = scope.ServiceProvider.GetRequiredService<INoteRepository>();
                NoteRepositorySeed.Seed(noteRepository);
            }
        }
    }
}
