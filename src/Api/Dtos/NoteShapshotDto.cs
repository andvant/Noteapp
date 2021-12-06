using Noteapp.Core.Entities;
using System;

namespace Noteapp.Api.Dtos
{
    public class NoteSnapshotDto
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        public NoteSnapshotDto(NoteSnapshot snapshot)
        {
            Id = snapshot.Id;
            NoteId = snapshot.NoteId;
            Text = snapshot.Text;
            Created = snapshot.Created;
        }
    }
}
