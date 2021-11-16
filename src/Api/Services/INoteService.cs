using Noteapp.Api.Entities;
using System.Collections.Generic;

namespace Noteapp.Api.Services
{
    public interface INoteService
    {
        Note Get(int userId, int noteId);
        IEnumerable<Note> GetAll(int userId, bool? archived);
        Note Create(int userId, string text);
        void BulkCreate(int userId, IEnumerable<string> texts);
        void Update(int userId, int noteId, string text);
        void Delete(int userId, int noteId);
        void Archive(int userId, int noteId);
        void Unarchive(int userId, int noteId);
        void Pin(int userId, int noteId);
        void Unpin(int userId, int noteId);
        void Lock(int userId, int noteId);
        void Unlock(int userId, int noteId);
        string Publish(int userId, int noteId);
        void Unpublish(int userId, int noteId);
        string GetPublishedNoteText(string url);
    }
}