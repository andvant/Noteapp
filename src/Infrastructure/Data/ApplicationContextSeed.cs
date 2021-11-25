using Noteapp.Core.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
{
    public static class ApplicationContextSeed
    {
        public static void Seed(ApplicationContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Notes.AddRange(GetNotes());
            context.AppUsers.AddRange(GetAppUsers());

            context.SaveChanges();
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

        private static List<Note> GetNotes()
        {
            const int userId = 1;

            return new()
            {
                new()
                {
                    Id = 1,
                    Text = "note 1",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                }
            };
        }
    }
}
