using Noteapp.Desktop.Models;

namespace Noteapp.Desktop.Session
{
    public class UserInfo
    {
        public string Email { get; set; } = "Anonymous";
        public string AccessToken { get; set; }
        public string EncryptionKey { get; set; }
        public bool EncryptionEnabled { get; set; } = false;
        public NotesSorting NotesSorting { get; set; } = NotesSorting.ByUpdatedAscending;
    }
}
