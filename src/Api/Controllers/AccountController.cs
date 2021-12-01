using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Filters;
using Noteapp.Core.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Noteapp.Api.Controllers
{
    [AccountExceptionFilter]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _userService.Register(dto.Email, dto.Password);
            return NoContent();
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            await _userService.ValidatePassword(dto.Email, dto.Password);

            return Ok(new
            {
                access_token = _tokenService.GenerateToken(dto.Email),
                email = dto.Email,
                encryption_salt = _userService.GetEncryptionSalt(dto.Email)
            });
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _userService.Delete(userId);

            return NoContent();
        }
    }
}
