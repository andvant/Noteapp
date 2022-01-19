using Microsoft.AspNetCore.Identity;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly AppUserService _appUserService;
        private readonly UserManager<AppUserIdentity> _userManager;

        public UserService(AppUserService appUserService, UserManager<AppUserIdentity> userManager)
        {
            _appUserService = appUserService ?? throw new ArgumentNullException(nameof(appUserService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // Creates both AppUserIdentity (only identity-related functionality) and AppUser (domain entity) with the same email
        public async Task Register(string email, string password)
        {
            var userIdentity = new AppUserIdentity(email);
            var result = await _userManager.CreateAsync(userIdentity, password);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException(string.Join("\n", result.Errors.Select(error => error.Description)));
            }

            await _appUserService.Create(email);
        }

        public async Task<AppUser> Get(string email, string password)
        {
            await ValidatePassword(email, password);
            return await _appUserService.Get(email);
        }

        public async Task Delete(int userId)
        {
            var user = await _appUserService.Get(userId);
            var userIdentity = await _userManager.FindByEmailAsync(user.Email);

            await _userManager.DeleteAsync(userIdentity);
            await _appUserService.Delete(userId);
        }

        private async Task ValidatePassword(string email, string password)
        {
            var userIdentity = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.CheckPasswordAsync(userIdentity, password))
            {
                throw new CredentialsNotValidException();
            }
        }
    }
}
