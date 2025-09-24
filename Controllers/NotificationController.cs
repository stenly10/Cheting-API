using Cheting.Data;
using Cheting.Models;
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
            var tasks = user.Conversations
                .Select(c => RabbitMQService.ConsumeMessage("conversation-" + c.Id.ToString() + "-username-" + user.Username))
                .ToList();
            var messages = await Task.WhenAll(tasks);

            return Ok(messages);
        }
    }
}