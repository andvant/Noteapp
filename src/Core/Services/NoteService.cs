using Noteapp.Core.Interfaces;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noteapp.Core.Services
{
    public class NoteService
    {
        private readonly INoteRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int MAX_BULK_NOTES = 20; // might want to move it somewhere else

        public NoteService(INoteRepository repository, IDateTimeProvider dateTimeProvider)
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
            var notes = _repository.Notes.Where(note => note.AuthorId == userId);
            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }
            return notes.ToList();
        }

        // just for testing
        public IEnumerable<Note> GetAllForAll()
        {
            return _repository.Notes.ToList();
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
            _repository.Notes.Remove(GetNote(userId, noteId));
        }

        public void Archive(int userId, int noteId)
        {
            GetNote(userId, noteId).Archived = true;
        }

        public void Unarchive(int userId, int noteId)
        {
            GetNote(userId, noteId).Archived = false;
        }

        public void Pin(int userId, int noteId)
        {
            GetNote(userId, noteId).Pinned = true;
        }

        public void Unpin(int userId, int noteId)
        {
            GetNote(userId, noteId).Pinned = false;
        }

        public void Lock(int userId, int noteId)
        {
            GetNote(userId, noteId).Locked = true;
        }

        public void Unlock(int userId, int noteId)
        {
            GetNote(userId, noteId).Locked = false;
        }

        public string Publish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = GenerateUrl();
            return note.PublicUrl;
        }

        public void Unpublish(int userId, int noteId)
        {
            GetNote(userId, noteId).PublicUrl = null;
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

        private void UpdateNote(Note note, string text)
        {
            note.Text = text;
            note.Updated = _dateTimeProvider.Now;
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
