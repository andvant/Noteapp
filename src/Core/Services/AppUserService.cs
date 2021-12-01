using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Security.Cryptography;

namespace Noteapp.Core.Services
{
    public class AppUserService
    {
        private readonly IAppUserRepository _repository;

        public AppUserService(IAppUserRepository repository)
        {
            _repository = repository;
        }

        public AppUser Create(string email)
        {
            var user = new AppUser()
            {
                Email = email,
                EncryptionSalt = GenerateEncryptionSalt()
            };

            _repository.Add(user);
            return user;
        }

        public AppUser Get(string email)
        {
            return _repository.FindByEmail(email);
        }

        public AppUser Get(int id)
        {
            return _repository.FindById(id);
        }

        public void Delete(int id)
        {
            var user = _repository.FindById(id);
            _repository.Delete(user);
        }

        private string GenerateEncryptionSalt()
        {
            var rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[8];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
