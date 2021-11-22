using System.Collections.Generic;

namespace Noteapp.Core.Entities
{
    public class AppUser : BaseEntity
    {
        // ASSUMED: unique for all AppUsers
        public string Email { get; set; }
        // temporarily store it in plaintext
        public string Password { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
