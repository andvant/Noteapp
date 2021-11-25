using Microsoft.AspNetCore.Identity;

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
