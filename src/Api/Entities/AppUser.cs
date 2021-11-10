using System.Collections.Generic;

namespace Noteapp.Api.Entities
{
    public class AppUser : BaseEntity
    {
        public string Email { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
