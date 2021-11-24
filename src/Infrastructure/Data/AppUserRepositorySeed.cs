using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
{
    public static class AppUserRepositorySeed
    {
        public static void Seed(IAppUserRepository repository)
        {
            repository.AppUsers = GetAppUsers();
        }

        private static List<AppUser> GetAppUsers()
        {
            return new()
            {
                new()
                {
                    Id = 1,
                    Email = "default@mail.com",
                    Notes = new List<Note>()
                },
                new()
                {
                    Id = 2,
                    Email = "user1@mail.com",
                    Notes = new List<Note>()
                },
                new()
                {
                    Id = 3,
                    Email = "user2@mail.com",
                    Notes = new List<Note>()
                }
            };
        }
    }
}
