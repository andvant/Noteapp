using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Infrastructure.Data
{
    public class EfNoteRepository : INoteRepository
    {
        private readonly ApplicationContext _context;

        public EfNoteRepository(ApplicationContext context)
        {
            _context = context;
        }

        // have to make two separate calls to SaveChanges, because inserting a new Note
        // with CurrentSnapshot property already set will produce a circular dependency
        public void Add(Note note)
        {
            using var transaction = _context.Database.BeginTransaction();

            var snapshot = note.CurrentSnapshot;
            note.CurrentSnapshot = null;

            _context.Notes.Add(note);
            _context.SaveChanges();

            note.CurrentSnapshot = snapshot;
            _context.SaveChanges();

            transaction.Commit();
        }

        // have to make two separate calls to SaveChanges, because inserting a new Note
        // with CurrentSnapshot property already set will produce a circular dependency
        public void AddRange(IEnumerable<Note> notes)
        {
            using var transaction = _context.Database.BeginTransaction();

            var snapshots = new List<NoteSnapshot>(notes.Count());
            foreach (var note in notes)
            {
                snapshots.Add(note.CurrentSnapshot);
                note.CurrentSnapshot = null;
            }

            _context.Notes.AddRange(notes);
            _context.SaveChanges();

            int i = 0;
            foreach (var note in notes)
            {
                note.CurrentSnapshot = snapshots[i++];
            }
            _context.SaveChanges();

            transaction.Commit();
        }

        public void Update(Note note)
        {
            _context.Entry(note).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Note note)
        {
            _context.Notes.Remove(note);
            _context.SaveChanges();
        }

        public IEnumerable<Note> GetAllForAuthor(int authorId, bool? archived)
        {
            var notes = _context.Notes.Where(note => note.AuthorId == authorId);
            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }
            return notes.Include(note => note.CurrentSnapshot).AsEnumerable();
        }

        public Note FindByPublicUrl(string url)
        {
            return _context.Notes.Where(note => note.PublicUrl == url).Include(note => note.CurrentSnapshot).SingleOrDefault();
        }

        public Note FindWithoutSnapshots(int id)
        {
            return _context.Notes.Find(id);
        }

        public Note FindWithCurrentSnapshot(int id)
        {
            return _context.Notes.Where(note => note.Id == id).Include(note => note.CurrentSnapshot).SingleOrDefault();
        }

        public Note FindWithAllSnapshots(int id)
        {
            return _context.Notes.Where(note => note.Id == id).Include(note => note.Snapshots).SingleOrDefault();
        }
    }
}
