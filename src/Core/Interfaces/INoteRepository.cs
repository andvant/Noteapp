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
        public Note Find(int id, bool includeSnapshots = false);
        public IEnumerable<Note> FindByAuthorId(int authorId);
        public Note FindByPublicUrl(string url);
        public IEnumerable<Note> GetAll(bool includeSnapshots = false);
    }
}
