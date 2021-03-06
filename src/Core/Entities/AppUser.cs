using System;
using System.Collections.Generic;

namespace Noteapp.Core.Entities
{
    public class AppUser : BaseEntity
    {
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<Note> Notes { get; set; }
        public string EncryptionSalt { get; set; }
    }
}
