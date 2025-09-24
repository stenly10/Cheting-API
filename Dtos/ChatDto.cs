namespace Cheting.Dtos
{
    public class ChatDto
    {
        public required Guid Id { get; set; }
        public required UserDto User { get; set; }
        public required string Message { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}