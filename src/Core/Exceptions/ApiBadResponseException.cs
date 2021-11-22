namespace Noteapp.Core.Exceptions
{
    public class ApiBadResponseException : NoteappException
    {
        public ApiBadResponseException(string responseMessage)
            : base($"Received unsuccessful response from the server:\n{responseMessage}")
        {
        }
    }
}
