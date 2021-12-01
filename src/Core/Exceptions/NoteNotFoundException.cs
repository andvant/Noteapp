namespace Noteapp.Core.Exceptions
{
    public class NoteNotFoundException : NoteappException
    {
        public NoteNotFoundException()
            : base($"Note with given id was not found")
        {
        }

        public NoteNotFoundException(string url)
            : base($"Note with public url {url} was not found")
        {
        }
    }
}
