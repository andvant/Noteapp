using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Infrastructure.Data
{
    public class EfAppUserRepository : IAppUserRepository
    {
        private readonly ApplicationContext _context;

        public EfAppUserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(AppUser user)
        {
            _context.AppUsers.Add(user);
            _context.SaveChanges();
        }

        public void Delete(AppUser user)
        {
            _context.AppUsers.Remove(user);
            _context.SaveChanges();
        }

        public AppUser FindByEmail(string email)
        {
            return _context.AppUsers.Where(user => user.Email == email).SingleOrDefault();
        }

        public AppUser FindById(int id)
        {
            return _context.AppUsers.Find(id);
        }

        public IEnumerable<AppUser> GetAll()
        {
            return _context.AppUsers.AsEnumerable();
        }

        //public void Update(AppUser user)
        //{
        //    _context.Entry(user).State = EntityState.Modified;
        //    _context.SaveChanges();
        //}

        //public void Delete(AppUser user)
        //{
        //    _context.AppUsers.Remove(user);
        //    _context.SaveChanges();
        //}



        //public IEnumerable<AppUser> Find(Expression<Func<AppUser, bool>> predicate)
        //{
        //    return _context.AppUsers.Where(predicate).AsEnumerable();
        //}


    }
}
