using System;
using System.Collections.Generic;

namespace Noteapp.Core.Entities
{
    public class Note : BaseEntity
    {
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public string PublicUrl { get; set; }
        public bool Published => PublicUrl != null;
        public DateTime Created { get; set; }
        public AppUser Author { get; set; }
        public int AuthorId { get; set; }
        public ICollection<NoteSnapshot> Snapshots { get; set; }
        public NoteSnapshot CurrentSnapshot { get; set; }
        public int? CurrentSnapshotId { get; set; }
        public string Text => CurrentSnapshot.Text;
        public DateTime Updated => CurrentSnapshot.Created;
    }
}
