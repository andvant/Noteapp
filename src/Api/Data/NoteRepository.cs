using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Api.Data
{
    public class NoteRepository
    {
        private readonly List<Note> _notes;
        private const int _userId = 1;

        public NoteRepository()
        {
            _notes = GetMockNotes();
        }

        public IEnumerable<Note> GetAll(int userId)
        {
            return _notes.FindAll(note => note.AuthorId == userId);
        }

        private List<Note> GetMockNotes()
        {

            var notes = new List<Note>()
            {
                new()
                {
                    Id = 1,
                    Text = "note 1",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = _userId
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = _userId
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = _userId
                }
            };

            return notes;
        }
    }
}
