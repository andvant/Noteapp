using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (InvalidNote(note, userId))
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

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }

        private int GenerateNewNoteId()
        {
            return _repository.Notes.Max(note => note?.Id) + 1 ?? 1;
        }
    }
}
