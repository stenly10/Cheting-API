using Cheting.Data;
using Cheting.Dtos;
using Cheting.Models;
using Cheting.Mappers;

namespace Cheting.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDBContext _context;

        public AuthServices(ApplicationDBContext context)
        {
            _context = context;
        }

        public User Register(RegisterRequestDto registerRequestDto)
        {
            var user = registerRequestDto.ToUser();
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }
    }
}