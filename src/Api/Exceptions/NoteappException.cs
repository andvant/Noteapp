using System;

namespace Noteapp.Api.Exceptions
{
    public class NoteappException : Exception
    {
        public NoteappException()
        {
        }

        public NoteappException(string message) : base(message)
        {
        }

        public NoteappException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
