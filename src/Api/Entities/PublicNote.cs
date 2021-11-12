using System;
using System.Text.Json.Serialization;

namespace Noteapp.Api.Entities
{
    public class PublicNote : BaseEntity
    {
        public string Url { get; set; }
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
