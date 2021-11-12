using System;
using System.Text.Json.Serialization;

namespace Noteapp.Api.Entities
{
    public class ArchivedNote : BaseEntity
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
