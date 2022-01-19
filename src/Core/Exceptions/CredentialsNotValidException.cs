namespace Noteapp.Core.Exceptions
{
    public class CredentialsNotValidException : NoteappException
    {
        public CredentialsNotValidException() : base($"Credentials not valid")
        {
        }
    }
}
