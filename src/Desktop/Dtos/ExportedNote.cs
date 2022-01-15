using Noteapp.Desktop.Models;
using System;

namespace Noteapp.Desktop.Dtos
{
    public class ExportedNote
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Text { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public bool Published { get; set; }

        public ExportedNote(Note note)
        {
            Id = note.Id;
            Created = note.Created;
            Updated = note.UpdatedLocal;
            Text = note.Text;
            Locked = note.Locked;
            Archived = note.Archived;
            Pinned = note.Pinned;
            Published = note.Published;
        }
    }
}
