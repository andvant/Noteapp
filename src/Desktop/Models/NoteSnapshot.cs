using System;

namespace Noteapp.Desktop.Models
{
    public class NoteSnapshot
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
