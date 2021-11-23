using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Core.Services
{
    // ASSUMED: that email is unique for all users
    public class AppUserService
    {
        private readonly IAppUserRepository _repository;

        public AppUserService(IAppUserRepository repository)
        {
            _repository = repository;
        }
        
        // TODO: check for a valid email and password
        public AppUser Create(string email, string password)
        {
            var appUser = new AppUser()
            {
                Id = GenerateNewAppUserId(),
                Email = email,
                Password = password,
                Notes = new List<Note>()
            };

            _repository.AppUsers.Add(appUser);

            return appUser;
        }

        public bool CredentialsValid(string email, string password)
        {
            var appUser = _repository.AppUsers.Find(user => user.Email == email);

            if (appUser?.Password != password)
            {
                return false;
            }

            return true;
        }

        public AppUser Get(string email)
        {
            return _repository.AppUsers.Find(user => user.Email == email);
        }

        // just for testing, should delete later
        public IEnumerable<AppUser> GetAll()
        {
            return _repository.AppUsers.ToList();
        }

        // TODO: make thread-safe
        private int GenerateNewAppUserId()
        {
            return _repository.AppUsers.Max(user => user?.Id) + 1 ?? 1;
        }
    }
}
