using Cheting.Data;
using Cheting.Dtos;
using Microsoft.AspNetCore.Mvc;
using Cheting.Mappers;
using Cheting.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Cheting.Models;

namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IAuthServices _authServices;


        public AuthenticationController(ApplicationDBContext context, IAuthServices authServices)
        {
            _context = context;
            _authServices = authServices;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return Conflict("Username already exists");
            }

            var user = _authServices.Register(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user.ToUserDto());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {

            var token = await _authServices.Login(dto);

            if (token == "")
            {
                return BadRequest("Wrong username/password");
            }
            
            return Ok(token);
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUserById([FromRoute] Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToUserDto());
        }

        [HttpGet("users/all")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users.Select(u => u.ToUserDto()).ToList());
        }
    }
}