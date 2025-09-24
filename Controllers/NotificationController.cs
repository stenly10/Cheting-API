using System.Text.Json;
using Cheting.Data;
using Cheting.Dtos;
using Cheting.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public NotificationController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotificationForUser([FromRoute] Guid userId)
        {
            var user = await _context.Users.Include(u => u.Conversations).FirstAsync(u => u.Id == userId);
            var conversations = user.Conversations.ToList();

            var messages = new List<ConversationNotificationDto>();
            
            foreach (var conversation in conversations)
            {
                var conversationNotificationDto = new ConversationNotificationDto
                {
                    ConversationId = conversation.Id
                };

                var conversationMessages = await RabbitMQService.ConsumeAllMessage("conversation-" + conversation.Id.ToString() + "-username-" + user.Username);

                foreach (var message in conversationMessages)
                {
                    var newMessage = JsonSerializer.Deserialize<ChatDto>(message);
                    conversationNotificationDto.Messages.Add(newMessage);
                }
                messages.Add(conversationNotificationDto);
            }

            return Ok(messages);
        }
    }
}