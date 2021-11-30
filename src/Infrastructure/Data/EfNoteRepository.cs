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

        public void Add(Note note, NoteSnapshot snapshot)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                _context.Notes.Add(note);
                _context.SaveChanges();

                note.CurrentSnapshot = snapshot;
                _context.SaveChanges();

                transaction.Commit();
            }
        }

        public void AddSnapshot(Note note, NoteSnapshot snapshot)
        {
            note.CurrentSnapshot = snapshot;
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Note> notes, IEnumerable<NoteSnapshot> snapshots)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                _context.Notes.AddRange(notes);
                _context.SaveChanges();

                var snapshotsEnumerator = snapshots.GetEnumerator();
                foreach (var note in notes)
                {
                    snapshotsEnumerator.MoveNext();
                    note.CurrentSnapshot = snapshotsEnumerator.Current;
                }
                _context.SaveChanges();

                transaction.Commit();
            }
        }

        public void Update(Note entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Note entity)
        {
            _context.Notes.Remove(entity);
            _context.SaveChanges();
        }

        // TODO: just for testing, remove later
        public IEnumerable<Note> GetAll()
        {
            return _context.Notes.Include(note => note.CurrentSnapshot).AsEnumerable();
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
