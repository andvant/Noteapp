using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Services
{
    public interface IAppUserService
    {
        AppUser Create(string email, string password);
        bool CredentialsValid(string email, string password);
        IEnumerable<AppUser> GetAll();
    }
}