using Noteapp.Desktop.Models;

namespace Noteapp.Desktop.Dtos
{
    public class NoteRequest
    {
        public string Text { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public bool Published { get; set; }

        public NoteRequest(Note note)
        {
            Text = note.Text;
            Locked = note.Locked;
            Archived = note.Archived;
            Pinned = note.Pinned;
            Published = note.Published;
        }
    }
}
