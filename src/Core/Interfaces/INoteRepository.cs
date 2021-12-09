using Noteapp.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface INoteRepository
    {
        public Task Add(Note note);
        public Task AddRange(IEnumerable<Note> notes);
        public Task Update(Note note);
        public Task Delete(Note note);

        public Task<IEnumerable<Note>> GetAllForAuthor(int authorId, bool? archived);
        public Task<Note> FindByPublicUrl(string url);
        public Task<Note> FindWithoutSnapshots(int id);
        public Task<Note> FindWithCurrentSnapshot(int id);
        public Task<Note> FindWithAllSnapshots(int id);
    }
}
