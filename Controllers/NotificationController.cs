using System.Text.Json;
using Cheting.Data;
using Cheting.Dtos;
using Cheting.RabbitMQ;
using Cheting.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly INotificationServices _notificationServices;
        public NotificationController(ApplicationDBContext context, INotificationServices notificationServices)
        {
            _context = context;
            _notificationServices = notificationServices;
        }

        [Authorize(Roles = "User")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotificationsForUser([FromRoute] Guid userId)
        {
            var user = await _context.Users.Include(u => u.Conversations).FirstAsync(u => u.Id == userId);
            var conversations = user.Conversations.ToList();

            var messages = await _notificationServices.GetNotificationsForUser(user, conversations);

            return Ok(messages);
        }
    }
}