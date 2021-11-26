using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface IAppUserRepository
    {
        public void Add(AppUser user);
        public AppUser Find(string email);
        public IEnumerable<AppUser> GetAll();
    }
}
