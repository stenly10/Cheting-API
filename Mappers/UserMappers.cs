using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Mappers
{
    public static class UserMappers
    {
        public static User ToUser(this RegisterRequestDto dto)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Name = dto.Name,
                Email = dto.Email
            };
        }
    }
}