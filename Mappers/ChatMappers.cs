using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Mappers
{
    public static class ChatMappers
    {
        public static Chat ToChat(this ChatRequestDto dto, User user, Conversation conversation)
        {
            return new Chat
            {
                Id = Guid.NewGuid(),
                User = user,
                Message = dto.Message,
                Conversation = conversation
            };
        }

        public static ChatDto ToChatResponseDto(this Chat chat)
        {
            return new ChatDto
            {
                Id = chat.Id,
                User = new UserDto
                {
                    Id = chat.User.Id,
                    Username = chat.User.Username,
                    Name = chat.User.Name,
                    Email = chat.User.Email
                },
                Message = chat.Message,
                CreatedAt = chat.CreatedAt
            };
        }
    }
}