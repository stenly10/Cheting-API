namespace Cheting.Models{
    public class Conversation
    {
        public required Guid Id { get; set; }
        public required List<Chat> Chats { get; set; } = new List<Chat>();
        public required List<User> Users { get; set; } = new List<User>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}