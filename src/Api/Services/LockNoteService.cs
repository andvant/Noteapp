using Noteapp.Api.Data;
using Noteapp.Api.Entities;

namespace Noteapp.Api.Services
{
    public class LockNoteService
    {
        private readonly NoteRepository _noteRepository;

        public LockNoteService(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public bool Lock(int userId, int noteId, bool @lock)
        {
            var note = _noteRepository.Notes.Find(note => note.Id == noteId);

            if (InvalidNote(note, userId))
            {
                return false;
            }

            note.Locked = @lock;
            return true;
        }

        private bool InvalidNote(Note note, int userId)
        {
            return note is null || note.AuthorId != userId;
        }
    }
}
