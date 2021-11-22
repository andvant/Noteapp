using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
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
