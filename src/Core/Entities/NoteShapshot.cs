using System;
using System.Text.Json.Serialization;

namespace Noteapp.Core.Entities
{
    public class NoteSnapshot : BaseEntity
    {
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
