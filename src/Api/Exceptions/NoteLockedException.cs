using System;

namespace Noteapp.Api.Exceptions
{
    public class NoteLockedException : NoteappException
    {
        public NoteLockedException(int noteId)
            : base($"The note with id {noteId} is locked and can't be modified")
        {
        }
    }
}
