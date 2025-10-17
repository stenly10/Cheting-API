namespace Cheting.Models
{
    public class User
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Email { get; set; }
        public string Role { get; set; } = UserRole.USER;
        public List<Conversation> Conversations { get; set; } = [];
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}