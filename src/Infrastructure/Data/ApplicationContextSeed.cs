using Noteapp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Data
{
    public static class ApplicationContextSeed
    {
        private static readonly DateTime _now = DateTime.UtcNow;

        public static async Task Seed(ApplicationContext context)
        {
            if (!context.AppUsers.Any())
            {
                await context.AppUsers.AddRangeAsync(GetAppUsers());
                await context.SaveChangesAsync();
            }

            if (!context.Notes.Any())
            {
                var notes = GetNotes();
                var snapshots = GetNoteSnapshots();
                await context.Notes.AddRangeAsync(notes);
                await context.NoteSnapshots.AddRangeAsync(snapshots);
                await context.SaveChangesAsync();

                notes[0].CurrentSnapshot = snapshots[0];
                notes[1].CurrentSnapshot = snapshots[1];
                notes[2].CurrentSnapshot = snapshots[2];
                await context.SaveChangesAsync();
            }
        }

        private static List<AppUser> GetAppUsers()
        {
            var rng = new Random(1);

            return new()
            {
                new()
                {
                    Id = 1,
                    Email = "default@mail.com",
                    RegistrationDate = _now,
                    EncryptionSalt = GenerateEncryptionSalt()
                },
                new()
                {
                    Id = 2,
                    Email = "user1@mail.com",
                    RegistrationDate = _now,
                    EncryptionSalt = GenerateEncryptionSalt()
                },
                new()
                {
                    Id = 3,
                    Email = "user2@mail.com",
                    RegistrationDate = _now,
                    EncryptionSalt = GenerateEncryptionSalt()
                }
            };

            string GenerateEncryptionSalt()
            {
                byte[] saltBytes = new byte[8];
                rng.NextBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private static List<Note> GetNotes()
        {
            const int userId = 1;

            var notes = new List<Note>()
            {
                new()
                {
                    Id = 1,
                    Created = _now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 2,
                    Created = _now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 3,
                    Created = _now,
                    AuthorId = userId
                }
            };

            return notes;
        }

        private static List<NoteSnapshot> GetNoteSnapshots()
        {
            return new List<NoteSnapshot>()
            {
                new()
                {
                    Id = 1,
                    Text = "note 1",
                    Created = _now,
                    NoteId = 1
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = _now,
                    NoteId = 2
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = _now,
                    NoteId = 3
                }
            };
        }
    }
}
