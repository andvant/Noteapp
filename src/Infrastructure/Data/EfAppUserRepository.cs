using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Data
{
    public class EfAppUserRepository : IAppUserRepository
    {
        private readonly ApplicationContext _context;

        public EfAppUserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Add(AppUser user)
        {
            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(AppUser user)
        {
            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> FindByEmail(string email)
        {
            return await _context.AppUsers.Where(user => user.Email == email).SingleOrDefaultAsync();
        }

        public async Task<AppUser> FindById(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }
    }
}
