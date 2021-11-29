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
            return GetNote(userId, noteId, true);
        }

        public IEnumerable<Note> GetAll(int userId, bool? archived)
        {
            var notes = _repository.FindByAuthorId(userId);

            if (archived.HasValue)
            {
                // TODO: filter in the query to the database
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

            var notes = new List<Note>();
            foreach (var text in texts)
            {
                notes.Add(CreateNote(userId, text));
            }
            _repository.AddRange(notes);
        }

        public Note Update(int userId, int noteId, string text)
        {
            var note = GetNote(userId, noteId);

            if (note.Locked)
            {
                throw new NoteLockedException(noteId);
            }

            AddNoteSnapshot(note, text);
            _repository.Update(note);
            return note;
        }

        public void Delete(int userId, int noteId)
        {
            _repository.Delete(GetNote(userId, noteId));
        }

        public Note Archive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = true;
            _repository.Update(note);
            return note;
        }

        public Note Unarchive(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Archived = false;
            _repository.Update(note);
            return note;
        }

        public Note Pin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = true;
            _repository.Update(note);
            return note;
        }

        public Note Unpin(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Pinned = false;
            _repository.Update(note);
            return note;
        }

        public Note Lock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = true;
            _repository.Update(note);
            return note;
        }

        public Note Unlock(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.Locked = false;
            _repository.Update(note);
            return note;
        }

        public Note Publish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = GenerateUrl();
            _repository.Update(note);
            return note;
        }

        public Note Unpublish(int userId, int noteId)
        {
            var note = GetNote(userId, noteId);
            note.PublicUrl = null;
            _repository.Update(note);
            return note;
        }

        public string GetPublishedNoteText(string url)
        {
            var note = _repository.FindByPublicUrl(url);
            return note?.Text ?? throw new NoteNotFoundException(url);
        }

        public NoteSnapshot GetSnapshot(int userId, int noteId, int snapshotId)
        {
            var note = GetNote(userId, noteId, true);

            var snapshot = note.Snapshots.Where(snapshot => snapshot.Id == snapshotId).SingleOrDefault();

            if (snapshot is null)
            {
                throw new SnapshotNotFoundException(noteId, snapshotId);
            }

            return snapshot;
        }

        public IEnumerable<NoteSnapshot> GetAllSnapshots(int userId, int noteId)
        {
            var note = GetNote(userId, noteId, true);

            return note.Snapshots;
        }

        private Note GetNote(int userId, int noteId, bool includeSnapshots = true)
        {
            var note = _repository.Find(noteId, includeSnapshots);
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
                AuthorId = userId,
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
            note.Snapshots = new List<NoteSnapshot>();
            note.Snapshots.Add(noteSnapshot);
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
