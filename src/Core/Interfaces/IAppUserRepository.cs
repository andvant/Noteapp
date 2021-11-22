using Noteapp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IAppUserRepository
    {
        List<AppUser> AppUsers { get; set; }
    }
}
