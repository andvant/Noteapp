﻿using Microsoft.AspNetCore.Identity;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Identity
{
    // ASSUMED: that email is unique for all users
    public class UserService : IUserService
    {
        private readonly AppUserService _appUserService;
        private readonly UserManager<AppUserIdentity> _userManager;

        public UserService(AppUserService appUserService, UserManager<AppUserIdentity> userManager)
        {
            _appUserService = appUserService;
            _userManager = userManager;
        }

        // TODO: check for a valid email and password
        // Creates both AppUserIdentity (only identity-related functionality) and AppUser (domain entity) with the same email
        public async Task Register(string email, string password)
        {
            var userIdentity = new AppUserIdentity(email);
            var result = await _userManager.CreateAsync(userIdentity, password);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException(string.Join("\n", result.Errors.Select(error => error.Description)));
            }

            _appUserService.Create(email);
        }

        public async Task ValidatePassword(string email, string password)
        {
            var userIdentity = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.CheckPasswordAsync(userIdentity, password))
            {
                throw new CredentialsNotValidException();
            }
        }

        public async Task Delete(int userId)
        {
            var user = _appUserService.Get(userId);
            var userIdentity = await _userManager.FindByEmailAsync(user.Email);

            await _userManager.DeleteAsync(userIdentity);
            _appUserService.Delete(userId);
        }

        public string GetEncryptionSalt(string email)
        {
            return _appUserService.Get(email).EncryptionSalt;
        }

        // just for testing, remove later
        public IEnumerable<AppUser> GetAllAppUsers()
        {
            return _appUserService.GetAll();
        }
    }
}