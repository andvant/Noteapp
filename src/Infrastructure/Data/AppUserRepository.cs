using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
{
    public class AppUserRepository : IAppUserRepository
    {
        public List<AppUser> AppUsers { get; set; }

        public AppUserRepository()
        {
            AppUsers = GetInMemoryAppUsers();
        }

        private List<AppUser> GetInMemoryAppUsers()
        {
            return new()
            {
                new()
                {
                    Id = 1,
                    Email = "user1@mail.com",
                    Password = "pass1",
                    Notes = new List<Note>()
                },
                new()
                {
                    Id = 2,
                    Email = "user2@mail.com",
                    Password = "pass2",
                    Notes = new List<Note>()
                },
                new()
                {
                    Id = 3,
                    Email = "user3@mail.com",
                    Password = "pass3",
                    Notes = new List<Note>()
                }
            };
        }
    }
}
