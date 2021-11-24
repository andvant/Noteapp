using Noteapp.Core.Interfaces;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noteapp.Core.Services
{
    public class NoteServiceNew
    {
        private readonly IRepository<Note> _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int MAX_BULK_NOTES = 20; // might want to move it somewhere else

        public NoteServiceNew(IRepository<Note> repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public Note Get(int userId, int noteId)
        {
            return GetNote(userId, noteId);
        }

        public IEnumerable<Note> GetAll(int userId, bool? archived)
        {
            var notes = _repository.Find(note => note.AuthorId == userId);

            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }

            return notes;
        }

        // just for testing
        public IEnumerable<Note> GetAllForAll()
        {
            return _repository.GetAll();
        }

        public Note Create(int userId, string text)
        {
            var note = CreateNote(userId, text);
            _repository.Add(note);
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
                _repository.Add(note);
            }
        }

        public void Update(int userId, int noteId, string text)
        {
            var note = GetNote(userId, noteId);

            if (note.Locked)
            {
                throw new NoteLockedException(noteId);
            }

            UpdateNote(note, text);
        }

        public void Delete(int userId, int noteId)
        {
            _repository.Delete(GetNote(userId, noteId));
        }

        public void Archive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = true;
            _repository.Update(note);
        }

        public void Unarchive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = false;
            _repository.Update(note);
        }

        public void Pin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = true;
            _repository.Update(note);
        }

        public void Unpin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = false;
            _repository.Update(note);
        }

        public void Lock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = true;
            _repository.Update(note);
        }

        public void Unlock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = false;
            _repository.Update(note);
        }

        public string Publish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = GenerateUrl();
            _repository.Update(note);
            return note.PublicUrl;
        }

        public void Unpublish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = null;
            _repository.Update(note);
        }

        public string GetPublishedNoteText(string url)
        {
            var note = _repository.Find(note => note.PublicUrl == url).SingleOrDefault();
            return note?.Text ?? throw new NoteNotFoundException(url);
        }

        private Note GetNote(int userId, int noteId)
        {
            var note = _repository.Find(noteId);
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException(userId, noteId);
            }
            return note;
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

        private void UpdateNote(Note note, string text)
        {
            note.Text = text;
            note.Updated = _dateTimeProvider.Now;
            _repository.Update(note);
        }

        // TODO: make thread-safe
        private int GenerateNewNoteId()
        {
            return _repository.GetAll().Max(note => note?.Id) + 1 ?? 1;
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
