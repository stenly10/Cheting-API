using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Mappers
{
    public static class ChatMappers
    {
        public static Chat ToChat(this ChatRequestDto dto, User user)
        {
            return new Chat
            {
                Id = Guid.NewGuid(),
                User = user,
                Message = dto.Message
            };
        }
    }
}