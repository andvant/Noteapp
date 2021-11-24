using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Noteapp.Api.Dtos;
using Noteapp.Core.Services;
using Noteapp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppUserService _appUserService;
        private readonly UserManager<AppUserIdentity> _userManager;

        public AccountController(AppUserService appUserService, UserManager<AppUserIdentity> userManager)
        {
            _appUserService = appUserService;
            _userManager = userManager;
        }

        // TODO: refactor error handling
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var userIdentity = new AppUserIdentity(dto.Email);

            var result = await _userManager.CreateAsync(userIdentity, dto.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors.Select(error => $"{error.Code}: {error.Description}")));
            }

            var user = _appUserService.Create(dto.Email);

            return Ok(user);

            //return Ok(_appUserService.Create(dto.Email, dto.Password));
        }

        // TODO: throw in service and catch exception in error handling filter
        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var userIdentity = await _userManager.FindByEmailAsync(dto.Email);
            if (!await _userManager.CheckPasswordAsync(userIdentity, dto.Password))
            {
                return Unauthorized("Credentials not valid");
            }

            // ASSUMED: emails of AppUser and AppUserIdentity are in sync
            var user = _appUserService.Get(dto.Email);

            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims);

            var jwt = new JwtSecurityTokenHandler().CreateEncodedJwt
            (
                issuer: "NoteappIssuer",
                audience: "NoteappAudience",
                subject: identity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                issuedAt: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123")), SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                access_token = jwt,
                email = identity.FindFirst(ClaimTypes.Email).Value
            });
        }

        // just for testing, should delete later
        [HttpGet("users")]
        public IActionResult GetAll()
        {
            return Ok(_appUserService.GetAll());
        }
    }
}
