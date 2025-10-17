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

        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}