namespace Cheting.Dtos
{
    public class RegisterRequestDto{
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}