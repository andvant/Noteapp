using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Noteapp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Data
{
    public class IdentityContext: IdentityDbContext<AppUserIdentity>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }
}
