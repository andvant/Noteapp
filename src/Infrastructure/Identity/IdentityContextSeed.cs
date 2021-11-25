﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Identity
{
    public static class IdentityContextSeed
    {
        public static async Task Seed(UserManager<AppUserIdentity> userManager)
        {
            foreach (var user in GetUsers())
            {
                if (!userManager.Users.Any()) await userManager.CreateAsync(user, "password");
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
