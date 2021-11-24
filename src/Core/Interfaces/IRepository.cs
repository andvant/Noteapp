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
        public T Find(int id);
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        public IEnumerable<T> GetAll();
    }
}
