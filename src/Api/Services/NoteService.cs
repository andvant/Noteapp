using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noteapp.Api.Services
{
    public class NoteService
    {
        private readonly NoteRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NoteService(NoteRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<Note> GetAll(int userId)
        {
            return _repository.Notes.FindAll(note => note.AuthorId == userId);
        }

        public Note Create(int userId, string text)
        {
            // Assumes that AppUser with Id of userId exists

            var note = new Note()
            {
                Created = _dateTimeProvider.Now,
                LastModified = _dateTimeProvider.Now,
                Id = GenerateNewNoteId(),
                Text = text,
                AuthorId = userId
            };

            _repository.Notes.Add(note);

            return note;
        }

        public Note TryGet(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId))
            {
                return null;
            }

            return note;
        }

        public bool TryUpdate(int userId, int noteId, string text)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId) || note.Locked)
            {
                return false;
            }

            note.Text = text;
            note.LastModified = _dateTimeProvider.Now;
            return true;
        }

        public bool TryDelete(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);
            if (InvalidNote(note, userId))
            {
                return false;
            }

            _repository.Notes.Remove(note);
            return true;
        }

        public bool Lock(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Locked = true;
            return true;
        }

        public bool Unlock(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Locked = false;
            return true;
        }

        public bool Archive(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Archived = true;
            return true;
        }

        public bool Unarchive(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Archived = false;
            return true;
        }

        public string Publish(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return null;
            }

            note.PublicUrl = GenerateUrl();
            return note.PublicUrl;
        }

        public bool Unpublish(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.PublicUrl = null;
            return true;
        }

        public string GetPublishedNoteText(string url)
        {
            var note = _repository.Notes.Find(note => note.PublicUrl == url);

            if (note is null)
            {
                return null;
            }

            return note.Text;
        }

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }

        private int GenerateNewNoteId()
        {
            return _repository.Notes.Max(note => note?.Id) + 1 ?? 1;
        }

        private string GenerateUrl()
        {
            const int LENGTH = 8;
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var url = new StringBuilder(LENGTH);

            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < LENGTH; i++)
            {
                url.Append(alphabet[random.Next(alphabet.Length)]);
            }

            return url.ToString();
        }
    }
}
