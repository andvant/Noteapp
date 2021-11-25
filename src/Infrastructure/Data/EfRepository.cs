using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Noteapp.Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationContext _context;

        public EfRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public T Find(int id, bool? includeSnapshots = null)
        {
            return _context.Set<T>().Where(note => note.Id == id).Include("Snapshots").SingleOrDefault();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, bool? includeSnapshots = null)
        {
            var entities = _context.Set<T>().Where(predicate);

            return EntitiesWithIncludedProperties(entities, includeSnapshots);
        }

        public IEnumerable<T> GetAll(bool? includeSnapshots = null)
        {
            var entities = _context.Set<T>();

            return EntitiesWithIncludedProperties(entities, includeSnapshots);
        }

        private IEnumerable<T> EntitiesWithIncludedProperties(IQueryable<T> entities, bool? includeSnapshots)
        {
            if (includeSnapshots.HasValue && includeSnapshots.Value)
            {
                entities = entities.Include("Snapshots");
            }

            return entities.AsEnumerable();
        }
    }
}
