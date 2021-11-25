using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Noteapp.Core.Services
{
    // ASSUMED: that email is unique for all users
    public class AppUserService
    {
        private readonly IRepository<AppUser> _repository;

        public AppUserService(IRepository<AppUser> repository)
        {
            _repository = repository;
        }

        // TODO: check for a valid email and password
        public AppUser Create(string email)
        {
            var appUser = new AppUser()
            {
                Id = GenerateNewAppUserId(),
                Email = email,
                Notes = new List<Note>()
            };

            _repository.Add(appUser);
            return appUser;
        }

        public AppUser Get(string email)
        {
            return _repository.Find(user => user.Email == email).Single();
        }

        // just for testing, remove later
        public IEnumerable<AppUser> GetAll()
        {
            return _repository.GetAll();
        }

        // TODO: make thread-safe
        private int GenerateNewAppUserId()
        {
            return _repository.GetAll().Max(user => user?.Id) + 1 ?? 1;
        }
    }
}
