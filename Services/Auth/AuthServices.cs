using Cheting.Data;
using Cheting.Dtos;
using Cheting.Models;
using Cheting.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cheting.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDBContext _context;
        private readonly JwtServices _jwtServices;

        public AuthServices(ApplicationDBContext context, JwtServices jwtServices)
        {
            _context = context;
            _jwtServices = jwtServices;
        }

        public User Register(RegisterRequestDto registerRequestDto)
        {

            var user = registerRequestDto.ToUser();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, registerRequestDto.Password);
            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public async Task<string> Login(LoginDto dto)
        {
            var user = await _context.Users.Where(u => u.Username == dto.Username).FirstOrDefaultAsync();

            if (user == null)
            {
                return "";
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            {
                return "";
            }

            return _jwtServices.GenerateToken(user.Id, user.Email, user.Role);
        }
    }
}