using Noteapp.Api.Entities;
using Noteapp.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Api.Data
{
    public class NoteService
    {
        private readonly List<Note> _notes;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NoteService(IDateTimeProvider dateTimeProvider)
        {
            _notes = GetMockNotes();
            _dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<Note> GetAll(int userId)
        {
            return _notes.FindAll(note => note.AuthorId == userId);
        }

        public Note Create(int userId, string text)
        {
            var note = new Note()
            {
                Created = _dateTimeProvider.Now,
                LastModified = _dateTimeProvider.Now,
                Id = _notes.Select(note => note.Id).Max() + 1,
                Text = text,
                AuthorId = userId
            };

            _notes.Add(note);

            return note;
        }

        public Note TryGet(int userId, int noteId)
        {
            var note = _notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId))
            {
                return null;
            }

            return note;
        }

        public bool TryUpdate(int userId, int noteId, string text)
        {
            var note = _notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Text = text;
            return true;
        }

        public bool TryDelete(int userId, int noteId)
        {
            var note = _notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId))
            {
                return false;
            }

            _notes.Remove(note);
            return true;
        }

        private List<Note> GetMockNotes()
        {
            const int userId = 1;

            var notes = new List<Note>()
            {
                new()
                {
                    Id = 1,
                    Text = "note 1",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    AuthorId = userId
                }
            };

            return notes;
        }

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }
    }
}
