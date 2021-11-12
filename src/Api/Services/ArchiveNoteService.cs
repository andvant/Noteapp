using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Api.Services
{
    public class ArchiveNoteService
    {
        private readonly ArchivedNoteRepository _archivedNoteRepository;
        private readonly NoteRepository _noteRepository;

        public ArchiveNoteService(ArchivedNoteRepository archivedNoteRepository, NoteRepository noteRepository)
        {
            _archivedNoteRepository = archivedNoteRepository;
            _noteRepository = noteRepository;
        }

        public IEnumerable<ArchivedNote> GetAll(int userId)
        {
            var notes = _noteRepository.Notes.FindAll(note => note.AuthorId == userId);

            // TODO: rewrite this
            return _archivedNoteRepository.ArchivedNotes.FindAll(an => notes.Any(note => note.Id == an.NoteId));
        }

        public bool Archive(int userId, int noteId)
        {
            var note = _noteRepository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            if (NoteAlreadyArchived(noteId))
            {
                return true;
            }

            var archivedNote = new ArchivedNote
            {
                Id = GenerateNewArchivedNoteId(),
                Note = note,
                NoteId = note.Id
            };

            _archivedNoteRepository.ArchivedNotes.Add(archivedNote);

            return true;
        }

        public bool Unarchive(int userId, int noteId)
        {
            var note = _noteRepository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            var archivedNote = _archivedNoteRepository.ArchivedNotes.Find(an => an.NoteId == noteId);

            if (archivedNote != null)
            {
                _archivedNoteRepository.ArchivedNotes.Remove(archivedNote);
            }

            return true;
            
        }

        private bool NoteAlreadyArchived(int noteId)
        {
            return _archivedNoteRepository.ArchivedNotes.Any(an => an.NoteId == noteId);
        }

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }

        private int GenerateNewArchivedNoteId()
        {
            return _archivedNoteRepository.ArchivedNotes.Max(an => an?.Id) + 1 ?? 1;
        }
    }
}
