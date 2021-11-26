using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;

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

        // TODO: check for a valid email
        public AppUser Create(string email)
        {
            var user = new AppUser()
            {
                Email = email
            };

            _repository.Add(user);
            return user;
        }

        public AppUser Get(string email)
        {
            return _repository.Find(email);
        }

        // just for testing, remove later
        public IEnumerable<AppUser> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
