using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface INoteRepository
    {
        public void Add(Note note, NoteSnapshot snapshot);
        public void AddSnapshot(Note note, NoteSnapshot snapshot);
        public void AddRange(IEnumerable<Note> notes, IEnumerable<NoteSnapshot> snapshots);
        public void Update(Note note);
        public void Delete(Note note);

        public IEnumerable<Note> GetAllForAuthor(int authorId, bool? archived);
        public Note FindByPublicUrl(string url);
        public Note FindWithAllSnapshots(int id);
        public Note FindWithoutSnapshots(int id);
        public Note FindWithCurrentSnapshot(int id);
    }
}
