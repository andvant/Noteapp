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
            if (!context.Notes.Any())
            {
                context.AppUsers.AddRange(GetAppUsers());

                var notes = GetNotes();
                var snapshots = GetNoteSnapshots();
                context.Notes.AddRange(notes);
                context.NoteSnapshots.AddRange(snapshots);
                context.SaveChanges();

                notes[0].CurrentSnapshot = snapshots[0];
                notes[1].CurrentSnapshot = snapshots[1];
                notes[2].CurrentSnapshot = snapshots[2];

                context.SaveChanges();
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
                    EncryptionSalt = GenerateEncryptionSalt()
                },
                new()
                {
                    Id = 2,
                    Email = "user1@mail.com",
                    EncryptionSalt = GenerateEncryptionSalt()
                },
                new()
                {
                    Id = 3,
                    Email = "user2@mail.com",
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
                    Created = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 2,
                    Created = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 3,
                    Created = DateTime.Now,
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
                    Created = DateTime.Now,
                    NoteId = 1
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = DateTime.Now,
                    NoteId = 2
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = DateTime.Now,
                    NoteId = 3
                }
            };
        }
    }
}
