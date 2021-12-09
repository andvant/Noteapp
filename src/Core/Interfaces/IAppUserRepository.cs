using Noteapp.Core.Entities;
using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IAppUserRepository
    {
        public Task Add(AppUser user);
        public Task Delete(AppUser user);
        public Task<AppUser> FindByEmail(string email);
        public Task<AppUser> FindById(int id);
    }
}
