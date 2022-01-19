namespace Noteapp.Core.Exceptions
{
    public class NoteLockedException : NoteappException
    {
        public NoteLockedException(int noteId)
            : base($"Note with id {noteId} is locked and can't be modified")
        {
        }
    }
}
