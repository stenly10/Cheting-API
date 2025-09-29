using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Mappers
{
    public static class ConversationMappers
    {
        public static ConversationResponseDto ToConversationResponseDto(this Conversation conversation)
        {
            return new ConversationResponseDto
            {
                Id = conversation.Id,
                Users = [.. conversation.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email
                })],
                CreatedAt = conversation.CreatedAt
            };
        }
    }
}