using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface INoteRepository
    {
        public void Add(Note note);
        public void AddRange(IEnumerable<Note> notes);
        public void Update(Note note);
        public void Delete(Note note);

        public IEnumerable<Note> GetAllForAuthor(int authorId, bool? archived);
        public Note FindByPublicUrl(string url);
        public Note FindWithoutSnapshots(int id);
        public Note FindWithCurrentSnapshot(int id);
        public Note FindWithAllSnapshots(int id);
    }
}
