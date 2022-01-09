using Noteapp.Core.Entities;
using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IUserService
    {
        public Task Register(string email, string password);
        public Task<AppUser> Get(string email, string password);
        public Task Delete(int userId);
    }
}