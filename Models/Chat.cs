namespace Cheting.Models
{
    public class Chat
    {
        public required Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Message { get; set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
