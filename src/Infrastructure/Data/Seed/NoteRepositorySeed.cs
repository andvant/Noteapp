using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data.Seed
{
    public static class NoteRepositorySeed
    {
        public static void Seed(INoteRepository repository)
        {
            repository.Notes = GetNotes();
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
