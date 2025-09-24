namespace Cheting.Dtos
{
    public class ConversationNotificationDto
    {
        public required Guid ConversationId { get; set; }
        public List<ChatDto> Messages { get; set; } = new List<ChatDto>();
    }
}