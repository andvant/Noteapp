using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
{
    public class AppUserRepository : IAppUserRepository
    {
        public List<AppUser> AppUsers { get; set; } = new();
    }
}
