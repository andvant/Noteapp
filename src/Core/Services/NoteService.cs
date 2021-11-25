using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noteapp.Core.Services
{
    public class NoteService
    {
        private readonly IRepository<Note> _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int MAX_BULK_NOTES = 20; // might want to move it somewhere else

        public NoteService(IDateTimeProvider dateTimeProvider, IRepository<Note> repository)
        {
            _dateTimeProvider = dateTimeProvider;
            _repository = repository;
        }

        public Note Get(int userId, int noteId)
        {
            return GetNote(userId, noteId);
        }

        public IEnumerable<Note> GetAll(int userId, bool? archived)
        {
            var notes = _repository.Find(note => note.AuthorId == userId, true);

            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }

            return notes;
        }

        // just for testing
        public IEnumerable<Note> GetAllForAll()
        {
            return _repository.GetAll(true);
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

            AddNoteSnapshot(note, text);
            _repository.Update(note);
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

        public NoteSnapshot GetSnapshot(int userId, int noteId, int snapshotId)
        {
            var note = GetNote(userId, noteId);

            var snapshot = note.Snapshots.Where(snapshot => snapshot.Id == snapshotId).SingleOrDefault();

            if (snapshot is null)
            {
                // TODO: create a custom exception
                throw new Exception("Snapshot was not found");
            }

            return snapshot;
        }

        public IEnumerable<NoteSnapshot> GetAllSnapshots(int userId, int noteId)
        {
            var note = _repository.Find(noteId, true);
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException(userId, noteId);
            }

            return note.Snapshots;
        }

        private Note GetNote(int userId, int noteId)
        {
            var note = _repository.Find(noteId, true);
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException(userId, noteId);
            }
            return note;
        }

        private Note CreateNote(int userId, string text)
        {
            var note = new Note()
            {
                Id = GenerateNewNoteId(),
                AuthorId = userId,
                Snapshots = new(),
                Created = _dateTimeProvider.Now,
            };
            AddNoteSnapshot(note, text);

            return note;
        }

        private void AddNoteSnapshot(Note note, string text)
        {
            var noteSnapshot = new NoteSnapshot()
            {
                Note = note,
                NoteId = note.Id,
                Created = _dateTimeProvider.Now,
                Text = text
            };
            note.Snapshots.Add(noteSnapshot);
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
