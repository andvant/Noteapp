namespace Noteapp.Core.Exceptions
{
    public class NoteNotFoundException : NoteappException
    {
        public NoteNotFoundException()
            : base($"Note was not found")
        {
        }
    }
}
