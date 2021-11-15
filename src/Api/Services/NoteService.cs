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
    // Assumes: that user with provided userId is authenticated (for all methods)
    public class NoteService
    {
        private readonly NoteRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int MAX_BULK_NOTES = 20; // might want to move it somewhere else

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
            var note = CreateNote(userId, text);
            _repository.Notes.Add(note);
            return note;
        }

        public void BulkCreate(int userId, IEnumerable<string> texts)
        {
            TooManyNotesException.ThrowIfTooManyNotes(texts.Count(), MAX_BULK_NOTES);

            foreach (var text in texts)
            {
                var note = CreateNote(userId, text);

                // TODO: add the whole list to the repository at once, but for that need to
                // change the implementation of GenerateNewNoteId() first
                _repository.Notes.Add(note);
            }
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
            note.Updated = _dateTimeProvider.Now;
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

        private Note CreateNote(int userId, string text)
        {
            return new Note()
            {
                Created = _dateTimeProvider.Now,
                Updated = _dateTimeProvider.Now,
                Id = GenerateNewNoteId(),
                Text = text,
                AuthorId = userId
            };
        }

        // TODO: make thread-safe
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
