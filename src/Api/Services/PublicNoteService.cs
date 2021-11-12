using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using System;
using System.Linq;

namespace Noteapp.Api.Services
{
    public class PublicNoteService
    {
        private readonly PublicNoteRepository _publicNoteRepository;
        private readonly NoteRepository _noteRepository;

        public PublicNoteService(PublicNoteRepository publicNoteRepository, NoteRepository noteRepository)
        {
            _publicNoteRepository = publicNoteRepository;
            _noteRepository = noteRepository;
        }

        public string GetNoteText(string url)
        {
            var publicNote = _publicNoteRepository.PublicNotes.Find(pn => pn.Url == url);

            if (publicNote is null)
            {
                return null;
            }

            var note = _noteRepository.Notes.Find(note => note.Id == publicNote.NoteId);

            return note.Text;
        }

        public string PublishNote(int userId, int noteId)
        {   
            var note = _noteRepository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return null;
            }

            var publicNote = new PublicNote()
            {
                Id = GenerateNewPublicNoteId(),
                Note = note,
                NoteId = note.Id,
                Url = GenerateUrl()
            };

            _publicNoteRepository.PublicNotes.Add(publicNote);

            return publicNote.Url;
    }

        private int GenerateNewPublicNoteId()
        {
            return _publicNoteRepository.PublicNotes.Max(pn => pn?.Id) + 1 ?? 1;
        }

        // TODO: generate proper url
        private string GenerateUrl()
        {
            return "url_" + new Random(Guid.NewGuid().GetHashCode()).Next(20_000).ToString();
        }

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }
    }
}
