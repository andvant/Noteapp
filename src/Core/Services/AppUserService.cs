using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Noteapp.Core.Services
{
    public class AppUserService
    {
        private readonly IAppUserRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AppUserService(IAppUserRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<AppUser> Create(string email)
        {
            await ValidateEmail(email);

            var user = new AppUser()
            {
                Email = email,
                EncryptionSalt = GenerateEncryptionSalt(),
                RegistrationDate = _dateTimeProvider.Now
            };

            await _repository.Add(user);
            return user;
        }

        public async Task<AppUser> Get(string email)
        {
            return await _repository.FindByEmail(email);
        }

        public async Task<AppUser> Get(int id)
        {
            return await _repository.FindById(id);
        }

        public async Task Delete(int id)
        {
            var user = await _repository.FindById(id);
            await _repository.Delete(user);
        }

        // will never fail because ASP.NET Core Identity performs its validation first
        private async Task ValidateEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
            {
                throw new UserRegistrationException("Provided email is invalid");
            }

            if (await _repository.FindByEmail(email) != null)
            {
                throw new UserRegistrationException("Provided email is already taken");
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
