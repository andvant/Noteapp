using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public class NoteRepository : INoteRepository
    {
        public List<Note> Notes { get; set; }

        public NoteRepository()
        {
            Notes = GetInMemoryNotes();
        }

        private List<Note> GetInMemoryNotes()
        {
            const int userId = 1;

            var notes = new List<Note>()
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

            return notes;
        }
    }
}
