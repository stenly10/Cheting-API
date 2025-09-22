namespace Cheting.Models
{
    public class Chat
    {
        public required Guid Id { get; set; }
        public required User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Message { get; set; }
    }
}
