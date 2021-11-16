namespace Noteapp.Api.Exceptions
{
    public class NoteNotFoundException : NoteappException
    {
        public NoteNotFoundException(int userId, int noteId)
            : base($"The user with id {userId} does not have a note with id {noteId}")
        {
        }

        public NoteNotFoundException(string url)
            : base($"The note with public url {url} does not exist")
        {
        }
    }
}
