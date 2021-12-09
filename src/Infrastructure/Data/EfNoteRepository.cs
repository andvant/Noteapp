using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task Add(Note note)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var snapshot = note.CurrentSnapshot;
            note.CurrentSnapshot = null;

            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();

            note.CurrentSnapshot = snapshot;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        // have to make two separate calls to SaveChanges, because inserting a new Note
        // with CurrentSnapshot property already set will produce a circular dependency
        public async Task AddRange(IEnumerable<Note> notes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var snapshots = new List<NoteSnapshot>(notes.Count());
            foreach (var note in notes)
            {
                snapshots.Add(note.CurrentSnapshot);
                note.CurrentSnapshot = null;
            }

            await _context.Notes.AddRangeAsync(notes);
            await _context.SaveChangesAsync();

            int i = 0;
            foreach (var note in notes)
            {
                note.CurrentSnapshot = snapshots[i++];
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public async Task Update(Note note)
        {
            _context.Entry(note).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Note>> GetAllForAuthor(int authorId, bool? archived)
        {
            var notes = _context.Notes.Where(note => note.AuthorId == authorId);
            if (archived.HasValue)
            {
                notes = notes.Where(note => note.Archived == archived.Value);
            }
            return await notes.Include(note => note.CurrentSnapshot).ToListAsync();
        }

        public async Task<Note> FindByPublicUrl(string url)
        {
            return await _context.Notes.Where(note => note.PublicUrl == url)
                .Include(note => note.CurrentSnapshot).SingleOrDefaultAsync();
        }

        public async Task<Note> FindWithoutSnapshots(int id)
        {
            return await _context.Notes.FindAsync(id);
        }

        public async Task<Note> FindWithCurrentSnapshot(int id)
        {
            return await _context.Notes.Where(note => note.Id == id)
                .Include(note => note.CurrentSnapshot).SingleOrDefaultAsync();
        }

        public async Task<Note> FindWithAllSnapshots(int id)
        {
            return await _context.Notes.Where(note => note.Id == id).Include(note => note.Snapshots).SingleOrDefaultAsync();
        }
    }
}
