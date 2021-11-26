using System;

namespace Noteapp.Desktop.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public string PublicUrl { get; set; }
        public bool Published => PublicUrl != null;
    }
}
