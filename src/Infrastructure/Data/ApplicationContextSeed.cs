using Noteapp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Infrastructure.Data
{
    public static class ApplicationContextSeed
    {
        public static void Seed(ApplicationContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (!context.Notes.Any())
            {
                context.Notes.AddRange(GetNotes());
                context.AppUsers.AddRange(GetAppUsers());
                context.SaveChanges();
            }

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

            var snapshots = new List<NoteSnapshot>()
            {   
                new()
                {
                    Text = "note 1",
                    Created = DateTime.Now
                },
                new()
                {
                    Text = "note 2",
                    Created = DateTime.Now
                },
                new()
                {
                    Text = "note 3",
                    Created = DateTime.Now
                }
            };

            var notes = new List<Note>()
            {
                new()
                {
                    Id = 1,
                    Created = DateTime.Now,
                    AuthorId = userId,
                    Snapshots = new List<NoteSnapshot>(new[] { snapshots[0] })
                },
                new()
                {
                    Id = 2,
                    Created = DateTime.Now,
                    AuthorId = userId,
                    Snapshots = new List<NoteSnapshot>(new[] { snapshots[1] })
                },
                new()
                {
                    Id = 3,
                    Created = DateTime.Now,
                    AuthorId = userId,
                    Snapshots = new List<NoteSnapshot>(new[] { snapshots[2] })
                }
            };

            return notes;
        }
    }
}
