using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
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

        public IEnumerable<Note> GetAll(int userId, bool? archived)
        {
            var notes = _repository.Notes.Where(note => note.AuthorId == userId);
            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }
            return notes.ToList();
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

        public Note Get(int userId, int noteId)
        {
            return GetNote(userId, noteId);
        }

        public void Update(int userId, int noteId, string text)
        {
            var note = GetNote(userId, noteId);

            if (note.Locked)
            {
                throw new NoteLockedException(noteId);
            }

            note.Text = text;
            note.LastModified = _dateTimeProvider.Now;
        }

        public void Delete(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            _repository.Notes.Remove(note);
        }

        public void Lock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = true;
        }

        public void Unlock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = false;
        }

        public void Archive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = true;
        }

        public void Unarchive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = false;
        }

        public void Pin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = true;
        }

        public void Unpin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = false;
        }

        public string Publish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);

            note.PublicUrl = GenerateUrl();
            return note.PublicUrl;
        }

        public void Unpublish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = null;
        }

        public string GetPublishedNoteText(string url)
        {
            var note = _repository.Notes.Find(note => note.PublicUrl == url);
            return note?.Text ?? throw new NoteNotFoundException(url);
        }

        private Note GetNote(int userId, int noteId)
        {
            var note = _repository.Notes.Find(note => note.Id == noteId && note.AuthorId == userId);
            return note ?? throw new NoteNotFoundException(userId, noteId);
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
