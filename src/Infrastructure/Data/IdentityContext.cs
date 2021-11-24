using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Noteapp.Infrastructure.Identity;

namespace Noteapp.Infrastructure.Data
{
    public class IdentityContext: IdentityDbContext<AppUserIdentity>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }
}
