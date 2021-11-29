﻿namespace Noteapp.Core.Exceptions
{
    public class UserRegistrationException : NoteappException
    {
        public UserRegistrationException(string errors) : base($"Failed to create a user:\r\n{errors}")
        {
        }
    }
}