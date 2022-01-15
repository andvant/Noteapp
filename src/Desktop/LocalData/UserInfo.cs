using Noteapp.Desktop.Models;
using System;

namespace Noteapp.Desktop.LocalData
{
    public class UserInfo
    {
        public string Email { get; set; } = "Anonymous";
        public string AccessToken { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string EncryptionKey { get; set; }
        public bool EncryptionEnabled { get; set; } = false;
        public NotesSorting NotesSorting { get; set; } = NotesSorting.ByUpdatedAscending;
    }
}
