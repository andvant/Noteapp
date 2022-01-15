using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Core.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Noteapp.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            await _userService.Register(request.Email, request.Password);
            return NoContent();
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userService.Get(request.Email, request.Password);

            return Ok(new UserInfoResponse
            (
                AccessToken: await _tokenService.GenerateToken(user.Email),
                Email: user.Email,
                EncryptionSalt: user.EncryptionSalt,
                RegistrationDate: user.RegistrationDate
            ));
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
