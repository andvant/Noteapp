using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public AppUser Author { get; set; }
        public int AuthorId { get; set; }
        [JsonIgnore]
        public List<NoteSnapshot> Snapshots { get; set; }
        [JsonIgnore]
        public NoteSnapshot CurrentSnapshot => Snapshots.Last();
        public int CurrentSnapshotId => CurrentSnapshot.Id;
        public string Text => CurrentSnapshot.Text;
        public DateTime Updated => CurrentSnapshot.Created;
    }
}
