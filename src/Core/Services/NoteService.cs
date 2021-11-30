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
            return GetNoteWithCurrentSnapshot(userId, noteId);
        }

        public IEnumerable<Note> GetAll(int userId, bool? archived)
        {
            var notes = _repository.GetAllForAuthor(userId, archived);

            return notes;
        }

        // TODO: just for testing, remove later
        public IEnumerable<Note> GetAllForAll()
        {
            return _repository.GetAll();
        }

        public Note Create(int userId, string text)
        {
            var note = CreateNote(userId);
            var snapshot = CreateSnapshot(note, text);

            _repository.Add(note, snapshot);

            return note;
        }

        public Note Update(int userId, int noteId, string text)
        {
            var note = GetNoteWithoutSnapshots(userId, noteId);

            if (note.Locked)
            {
                throw new NoteLockedException(noteId);
            }

            var snapshot = CreateSnapshot(note, text);

            _repository.AddSnapshot(note, snapshot);
            return note;
        }

        public void BulkCreate(int userId, IEnumerable<string> texts)
        {
            TooManyNotesException.ThrowIfTooManyNotes(texts.Count(), MAX_BULK_NOTES);

            var notes = new List<Note>();
            var snapshots = new List<NoteSnapshot>();

            foreach (var text in texts)
            {
                var note = CreateNote(userId);
                var snapshot = CreateSnapshot(note, text);

                notes.Add(note);
                snapshots.Add(snapshot);
            }

            _repository.AddRange(notes, snapshots);
        }

        public void Delete(int userId, int noteId)
        {
            var note = GetNoteWithoutSnapshots(userId, noteId);
            _repository.Delete(note);
        }

        public Note Archive(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Archived = true;
            _repository.Update(note);
            return note;
        }

        public Note Unarchive(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Archived = false;
            _repository.Update(note);
            return note;
        }

        public Note Pin(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Pinned = true;
            _repository.Update(note);
            return note;
        }

        public Note Unpin(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Pinned = false;
            _repository.Update(note);
            return note;
        }

        public Note Lock(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Locked = true;
            _repository.Update(note);
            return note;
        }

        public Note Unlock(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.Locked = false;
            _repository.Update(note);
            return note;
        }

        public Note Publish(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.PublicUrl = GenerateUrl();
            _repository.Update(note);
            return note;
        }

        public Note Unpublish(int userId, int noteId)
        {
            var note = GetNoteWithCurrentSnapshot(userId, noteId);
            note.PublicUrl = null;
            _repository.Update(note);
            return note;
        }

        public string GetPublishedNoteText(string url)
        {
            var note = _repository.FindByPublicUrl(url);
            return note?.Text ?? throw new NoteNotFoundException(url);
        }

        // TODO: consider removal of this method (no one needs it really)
        public NoteSnapshot GetSnapshot(int userId, int noteId, int snapshotId)
        {
            var note = GetNoteWithSnapshots(userId, noteId);

            var snapshot = note.Snapshots.Where(snapshot => snapshot.Id == snapshotId).SingleOrDefault();

            if (snapshot is null)
            {
                throw new SnapshotNotFoundException(noteId, snapshotId);
            }

            return snapshot;
        }

        public IEnumerable<NoteSnapshot> GetAllSnapshots(int userId, int noteId)
        {
            var note = GetNoteWithSnapshots(userId, noteId);

            return note.Snapshots.OrderBy(snapshot => snapshot.Created);
        }

        private Note GetNoteWithSnapshots(int userId, int noteId)
        {
            var note = _repository.FindWithAllSnapshots(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private Note GetNoteWithoutSnapshots(int userId, int noteId)
        {
            var note = _repository.FindWithoutSnapshots(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private Note GetNoteWithCurrentSnapshot(int userId, int noteId)
        {
            var note = _repository.FindWithCurrentSnapshot(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private void ValidateFound(Note note, int userId)
        {
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException(userId, note.Id);
            }
        }

        private Note CreateNote(int userId)
        {
            return new Note()
            {
                AuthorId = userId,
                Created = _dateTimeProvider.Now,
            };
        }

        private NoteSnapshot CreateSnapshot(Note note, string text)
        {
            return new NoteSnapshot()
            {
                Created = _dateTimeProvider.Now,
                Text = text,
                Note = note
            };
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
