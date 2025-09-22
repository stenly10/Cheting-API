namespace Cheting.Dtos
{
    public class ChatRequestDto
    {
        public required Guid UserId { get; set; }
        public required string Message { get; set; }
    }
}