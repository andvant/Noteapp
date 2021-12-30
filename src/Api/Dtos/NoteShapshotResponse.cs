using Noteapp.Core.Entities;
using System;

namespace Noteapp.Api.Dtos
{
    public class NoteSnapshotResponse
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        public NoteSnapshotResponse(NoteSnapshot snapshot)
        {
            Id = snapshot.Id;
            NoteId = snapshot.NoteId;
            Text = snapshot.Text;
            Created = snapshot.Created;
        }
    }
}
