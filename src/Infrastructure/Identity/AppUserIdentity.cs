using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Identity
{
    public class AppUserIdentity : IdentityUser
    {
        public AppUserIdentity(string email)
        {
            Email = email;
            UserName = email;
        }
    }
}
