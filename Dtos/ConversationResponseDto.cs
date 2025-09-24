using Cheting.Models;

namespace Cheting.Dtos
{
    public class ConversationResponseDto
    {
        public required Guid Id { get; set; }
        public required List<UserDto> Users { get; set; }
    }
}