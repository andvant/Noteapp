using Microsoft.AspNetCore.Identity;
using Noteapp.Infrastructure.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Data.Seed
{
    public static class IdentityContextSeed
    {
        public static async Task Seed(UserManager<AppUserIdentity> userManager)
        {
            foreach (var user in GetUsers())
            {
                await userManager.CreateAsync(user, "password");
            }
        }

        private static IEnumerable<AppUserIdentity> GetUsers()
        {
            return new AppUserIdentity[]
            {
                new("default@mail.com"),
                new("user1@mail.com"),
                new("user2@mail.com")
            };
        }
    }
}
