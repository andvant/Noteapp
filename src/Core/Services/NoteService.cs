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
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
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

        public async Task<Note> Create(int userId, string text)
        {
            var note = CreateNote(userId);
            note.CurrentSnapshot = CreateSnapshot(note, text);

            await _repository.Add(note);
            return note;
        }

        public async Task<Note> CreateNew(int userId, UpdateNoteDtoNew noteDto)
        {
            var note = CreateNoteNew(userId, noteDto);
            note.CurrentSnapshot = CreateSnapshot(note, noteDto.Text);

            await _repository.Add(note);
            return note;
        }

        public async Task<Note> Update(int userId, int noteId, string text)
        {
            var note = await GetNoteWithoutSnapshots(userId, noteId);

            if (note.Locked)
            {
                throw new NoteLockedException(noteId);
            }

            note.CurrentSnapshot = CreateSnapshot(note, text);

            await _repository.Update(note);
            return note;
        }

        public async Task<Note> UpdateNew(int userId, int noteId, UpdateNoteDtoNew noteDto)
        {
            var note = await GetNoteWithCurrentSnapshot(userId, noteId);

            if (note.Text != noteDto.Text)
            {
                if (note.Locked) throw new NoteLockedException(noteId);
                note.CurrentSnapshot = CreateSnapshot(note, noteDto.Text);
            }

            note.Pinned = noteDto.Pinned;
            note.Locked = noteDto.Locked;
            note.Archived = noteDto.Archived;
            note.PublicUrl = note.Published != noteDto.Published
                ? (noteDto.Published ? GenerateUrl() : null)
                : note.PublicUrl;

            await _repository.Update(note);
            return note;
        }

        public async Task BulkCreate(int userId, IEnumerable<string> texts)
        {
            TooManyNotesException.ThrowIfTooManyNotes(texts.Count(), Constants.MAX_BULK_NOTES);

            var notes = new List<Note>();
            foreach (var text in texts)
            {
                var note = CreateNote(userId);
                note.CurrentSnapshot = CreateSnapshot(note, text);

                notes.Add(note);
            }

            await _repository.AddRange(notes);
        }

        public async Task BulkCreateNew(int userId, IEnumerable<UpdateNoteDtoNew> noteDtos)
        {
            TooManyNotesException.ThrowIfTooManyNotes(noteDtos.Count(), Constants.MAX_BULK_NOTES);

            var notes = new List<Note>();
            foreach (var noteDto in noteDtos)
            {
                var note = CreateNoteNew(userId, noteDto);
                note.CurrentSnapshot = CreateSnapshot(note, noteDto.Text);

                notes.Add(note);
            }

            await _repository.AddRange(notes);
        }

        public async Task Delete(int userId, int noteId)
        {
            var note = await GetNoteWithoutSnapshots(userId, noteId);
            await _repository.Delete(note);
        }

        public async Task<Note> Archive(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Archived = true);
        }

        public async Task<Note> Unarchive(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Archived = false);
        }

        public async Task<Note> Pin(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Pinned = true);
        }

        public async Task<Note> Unpin(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Pinned = false);
        }

        public async Task<Note> Lock(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Locked = true);
        }

        public async Task<Note> Unlock(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.Locked = false);
        }

        public async Task<Note> Publish(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.PublicUrl = GenerateUrl());
        }

        public async Task<Note> Unpublish(int userId, int noteId)
        {
            return await ModifyNote(userId, noteId, note => note.PublicUrl = null);
        }

        public async Task<string> GetPublishedNoteText(string url)
        {
            var note = await _repository.FindByPublicUrl(url);
            return note?.Text ?? throw new NoteNotFoundException(url);
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

        private async Task<Note> ModifyNote(int userId, int noteId, Action<Note> modification)
        {
            var note = await GetNoteWithCurrentSnapshot(userId, noteId);
            modification(note);
            await _repository.Update(note);
            return note;
        }

        private void ValidateFound(Note note, int userId)
        {
            if (note is null || note.AuthorId != userId)
            {
                throw new NoteNotFoundException();
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

        private Note CreateNoteNew(int userId, UpdateNoteDtoNew dto)
        {
            return new Note()
            {
                AuthorId = userId,
                Created = _dateTimeProvider.Now,
                Locked = dto.Locked,
                Pinned = dto.Pinned,
                Archived = dto.Archived,
                PublicUrl = dto.Published ? GenerateUrl() : null
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
