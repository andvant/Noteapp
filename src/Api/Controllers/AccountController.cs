using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Noteapp.Api.Dtos;
using Noteapp.Api.Filters;
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
    [AccountExceptionFilter]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AccountController(UserService userService, TokenService tokenService)
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

        // TODO: throw in service and catch exception in error handling filter
        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            await _userService.ValidatePassword(dto.Email, dto.Password);

            return Ok(new
            { 
                access_token = _tokenService.GenerateToken(dto.Email),
                email = dto.Email
            });
        }

        // just for testing, remove later
        [HttpGet("users")]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetAllAppUsers());
        }
    }
}
