﻿using Noteapp.Core.Exceptions;

namespace Noteapp.Desktop.Exceptions
{
    public class ApiBadResponseException : NoteappException
    {
        public ApiBadResponseException(string responseMessage)
            : base($"Received unsuccessful response from the server:\n{responseMessage}")
        {
        }
    }
}
