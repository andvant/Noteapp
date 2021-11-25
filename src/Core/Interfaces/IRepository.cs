using Noteapp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Noteapp.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public T Find(int id, bool? includeSnapshots = null);
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, bool? includeSnapshots = null);
        public IEnumerable<T> GetAll(bool? includeSnapshots = null);
    }
}
