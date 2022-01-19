using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly AppUserService _appUserService;
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(AppUserService appUserService, IOptions<JwtSettings> jwtSettings,
            IDateTimeProvider dateTimeProvider)
        {
            _appUserService = appUserService ?? throw new ArgumentNullException(nameof(appUserService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> GenerateToken(string userEmail)
        {
            var user = await _appUserService.Get(userEmail);

            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims);

            var jwt = new JwtSecurityTokenHandler().CreateEncodedJwt
            (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                subject: identity,
                notBefore: _dateTimeProvider.Now,
                expires: _dateTimeProvider.Now.Add(TimeSpan.FromMinutes(_jwtSettings.LifetimeMinutes)),
                issuedAt: _dateTimeProvider.Now,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)), SecurityAlgorithms.HmacSha256)
            );

            return jwt;
        }
    }
}
