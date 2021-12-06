using Microsoft.IdentityModel.Tokens;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Noteapp.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly AppUserService _appUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(AppUserService appUserService, IDateTimeProvider dateTimeProvider)
        {
            _appUserService = appUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public string GenerateToken(string userEmail)
        {
            var user = _appUserService.Get(userEmail);

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
                notBefore: _dateTimeProvider.Now,
                expires: _dateTimeProvider.Now.Add(TimeSpan.FromDays(1)),
                issuedAt: _dateTimeProvider.Now,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123")), SecurityAlgorithms.HmacSha256)
            );

            return jwt;
        }
    }
}
