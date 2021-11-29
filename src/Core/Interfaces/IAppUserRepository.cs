using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface IAppUserRepository
    {
        public void Add(AppUser user);
        public void Delete(AppUser user);
        public AppUser FindByEmail(string email);
        public AppUser FindById(int id);
        public IEnumerable<AppUser> GetAll();
    }
}
