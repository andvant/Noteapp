using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Noteapp.Core.Services
{
    public class AppUserService
    {
        private readonly IAppUserRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AppUserService(IAppUserRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public AppUser Create(string email)
        {
            ValidateEmail(email);

            var user = new AppUser()
            {
                Email = email,
                EncryptionSalt = GenerateEncryptionSalt(),
                RegistrationDate = _dateTimeProvider.Now
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

        // will never fail because ASP.NET Core Identity performs its validation first
        private void ValidateEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
            {
                throw new UserRegistrationException("Provided email is invalid.");
            }

            if (_repository.FindByEmail(email) != null)
            {
                throw new UserRegistrationException("Provided email is already taken.");
            }
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
