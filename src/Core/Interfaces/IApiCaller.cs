using Noteapp.Core.Entities;
using Noteapp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IApiCaller
    {
        string AccessToken { get; set; }

        Task BulkCreateNotes(IEnumerable<Note> notes);
        Task CreateNote();
        Task DeleteNote(int noteId);
        Task EditNote(int noteId, string text);
        Task<IEnumerable<Note>> GetAllNotes();
        Task<IEnumerable<Note>> GetArchivedNotes();
        Task<IEnumerable<Note>> GetNonArchivedNotes();
        Task<UserInfo> Login(string email, string password);
        Task Register(string email, string password);
        Task ToggleArchived(int noteId, bool archived);
        Task ToggleLocked(int noteId, bool locked);
        Task TogglePinned(int noteId, bool pinned);
        Task TogglePublished(int noteId, bool published);
    }
}