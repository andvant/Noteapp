using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Core.Services;

namespace Noteapp.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public AccountController(IAppUserService appUserService)
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
            if (_appUserService.CredentialsValid(dto.Email, dto.Password))
            {
                return NoContent();
            }
            else
            {
                return Unauthorized();
            }
        }

        // just for testing, should delete later
        [HttpGet("users")]
        public IActionResult GetAll()
        {
            return Ok(_appUserService.GetAll());
        }
    }
}
