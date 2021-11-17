using System;
using Noteapp.Core.Exceptions;

namespace Noteapp.Desktop.Exceptions
{
    public class ApiConnectionException : NoteappException
    {
        public ApiConnectionException(Exception innerException) 
            : base("Could not connect to the server", innerException)
        {
        }
    }
}
