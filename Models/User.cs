namespace Cheting.Models
{
    public class User
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Email { get; set; }
        public List<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}