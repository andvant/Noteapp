using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Noteapp.Api.Dtos;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Noteapp.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppUserService _appUserService;

        public AccountController(AppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            return Ok(_appUserService.Create(dto.Email, dto.Password));
        }

        // TODO: throw in service and catch exception in error handling filter
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            if (!_appUserService.CredentialsValid(dto.Email, dto.Password))
            {
                return Unauthorized("Credentials not valid");
            }

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
