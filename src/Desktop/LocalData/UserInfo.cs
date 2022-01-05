﻿namespace Noteapp.Desktop.Session
{
    public class UserInfo
    {
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public string EncryptionKey { get; set; }
        public bool EncryptionEnabled { get; set; } = true;
    }
}