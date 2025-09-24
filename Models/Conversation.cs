namespace Cheting.Models{
    public class Conversation
    {
        public required Guid Id { get; set; }
        public ICollection <Chat> Chats { get; set; } = new List<Chat>();
        public List<User> Users { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}