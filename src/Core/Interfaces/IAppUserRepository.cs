using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface IAppUserRepository
    {
        List<AppUser> AppUsers { get; set; }
    }
}
