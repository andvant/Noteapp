namespace Noteapp.Core.Exceptions
{
    public class UserRegistrationException : NoteappException
    {
        public UserRegistrationException(string error) : base(error)
        {
        }
    }
}
