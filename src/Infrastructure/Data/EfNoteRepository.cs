using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Noteapp.Infrastructure.Data
{
    public class EfNoteRepository : INoteRepository
    {
        private readonly ApplicationContext _context;

        public EfNoteRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(Note entity)
        {
            _context.Notes.Add(entity);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Note> notes)
        {
            _context.Notes.AddRange(notes);
            _context.SaveChanges();
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

        public Note Find(int id, bool includeSnapshots = false)
        {
            var note = _context.Notes.Where(note => note.Id == id);

            return IncludeSnapshots(note, includeSnapshots).SingleOrDefault();
        }

        public IEnumerable<Note> FindByAuthorId(int authorId)
        {
            return _context.Notes.Where(note => note.AuthorId == authorId)
                .Include(note => note.Snapshots).AsEnumerable();
        }

        public Note FindByPublicUrl(string url)
        {
            return _context.Notes.Where(note => note.PublicUrl == url).Include(note => note.Snapshots).SingleOrDefault();
        }

        public IEnumerable<Note> GetAll(bool includeSnapshots = false)
        {
            var entities = _context.Notes;

            return IncludeSnapshots(entities, includeSnapshots).AsEnumerable();
        }

        private IQueryable<Note> IncludeSnapshots(IQueryable<Note> entities, bool includeSnapshots)
        {
            if (includeSnapshots)
            {
                entities = entities.Include(note => note.Snapshots);
            }

            return entities;
        }

    }
}
