using Cheting.Data;
using Cheting.Models;
using Cheting.Dtos;
using Microsoft.AspNetCore.Mvc;
using Cheting.Mappers;

namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AuthenticationController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return Conflict("Username already exists");
            }

            var user = dto.ToUser();
            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user.ToUserDto());
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