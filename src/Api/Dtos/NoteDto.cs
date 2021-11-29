using Noteapp.Core.Entities;
using System;

namespace Noteapp.Api.Dtos
{
    public class NoteDto
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
        public bool Published { get; set; }

        public NoteDto(Note note)
        {
            Id = note.Id;
            AuthorId = note.AuthorId;
            Text = note.Text;
            Created = note.Created;
            Updated = note.Updated;
            Locked = note.Locked;
            Archived = note.Archived;
            Pinned = note.Pinned;
            PublicUrl = note.PublicUrl;
            Published = note.Published;
        }
    }
}
