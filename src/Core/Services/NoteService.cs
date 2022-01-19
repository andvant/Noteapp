using Noteapp.Core.Dtos;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Core.Services
{
    public class NoteService
    {
        private readonly INoteRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NoteService(INoteRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<Note> Get(int userId, int noteId)
        {
            return await GetNoteWithCurrentSnapshot(userId, noteId);
        }

        public async Task<IEnumerable<Note>> GetAll(int userId, bool? archived)
        {
            var notes = await _repository.GetAllForAuthor(userId, archived);
            return notes;
        }

        public async Task<Note> Create(int userId, NoteRequest request)
        {
            var note = CreateNote(userId, request);
            note.CurrentSnapshot = CreateSnapshot(note, request.Text);

            await _repository.Add(note);
            return note;
        }

        public async Task BulkCreate(int userId, IEnumerable<NoteRequest> requests)
        {
            TooManyNotesException.ThrowIfTooManyNotes(requests.Count(), Constants.MAX_BULK_NOTES);

            var notes = new List<Note>();
            foreach (var request in requests)
            {
                var note = CreateNote(userId, request);
                note.CurrentSnapshot = CreateSnapshot(note, request.Text);

                notes.Add(note);
            }

            await _repository.AddRange(notes);
        }

        public async Task<Note> Update(int userId, int noteId, NoteRequest request)
        {
            var note = await GetNoteWithCurrentSnapshot(userId, noteId);

            if (note.Text != request.Text)
            {
                if (note.Locked) throw new NoteLockedException(noteId);
                note.CurrentSnapshot = CreateSnapshot(note, request.Text);
            }

            note.Pinned = request.Pinned;
            note.Locked = request.Locked;
            note.Archived = request.Archived;
            note.PublicUrl = note.Published != request.Published
                ? (request.Published ? GenerateUrl() : null)
                : note.PublicUrl;

            await _repository.Update(note);
            return note;
        }

        public async Task Delete(int userId, int noteId)
        {
            var note = await GetNoteWithoutSnapshots(userId, noteId);
            await _repository.Delete(note);
        }

        public async Task<string> GetPublishedNoteText(string url)
        {
            var note = await _repository.FindByPublicUrl(url);
            return note?.Text ?? throw new NoteNotFoundException();
        }

        public async Task<IEnumerable<NoteSnapshot>> GetAllSnapshots(int userId, int noteId)
        {
            var note = await GetNoteWithSnapshots(userId, noteId);
            return note.Snapshots.OrderBy(snapshot => snapshot.Created);
        }

        private async Task<Note> GetNoteWithSnapshots(int userId, int noteId)
        {
            var note = await _repository.FindWithAllSnapshots(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private async Task<Note> GetNoteWithoutSnapshots(int userId, int noteId)
        {
            var note = await _repository.FindWithoutSnapshots(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private async Task<Note> GetNoteWithCurrentSnapshot(int userId, int noteId)
        {
            var note = await _repository.FindWithCurrentSnapshot(noteId);
            ValidateFound(note, userId);
            return note;
        }

        private void ValidateFound(Note note, int userId)
        {
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException();
            }
        }

        private Note CreateNote(int userId, NoteRequest request)
        {
            return new Note()
            {
                AuthorId = userId,
                Created = _dateTimeProvider.Now,
                Locked = request.Locked,
                Pinned = request.Pinned,
                Archived = request.Archived,
                PublicUrl = request.Published ? GenerateUrl() : null
            };
        }

        private NoteSnapshot CreateSnapshot(Note note, string text)
        {
            TextTooLongException.ThrowIfTextTooLong(text.Length, Constants.MAX_TEXT_LENGTH);

            return new NoteSnapshot()
            {
                Created = _dateTimeProvider.Now,
                Text = text,
                Note = note
            };
        }

        private string GenerateUrl()
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var url = new StringBuilder(Constants.PUBLIC_URL_LENGTH);
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < Constants.PUBLIC_URL_LENGTH; i++)
            {
                url.Append(alphabet[random.Next(alphabet.Length)]);
            }

            return url.ToString();
        }
    }
}
