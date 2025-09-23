using Microsoft.AspNetCore.Mvc;
using Cheting.Data;
using Cheting.Models;
using Cheting.Dtos;
using Cheting.Mappers;
using Cheting.RabbitMQ;
using System;
using System.Threading.Tasks;
using System.Text.Json;


namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/conversation")]
    public class ConversationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ConversationController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetConversationById(Guid id)
        {
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
            {
                return NotFound();
            }
            return Ok(conversation);
        }

        [HttpGet("all")]
        public IActionResult GetAllConversations()
        {
            var conversations = _context.Conversations.ToList();
            return Ok(conversations);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetAllConversationsForUser(Guid userId)
        {
            var conversations = _context.Conversations
                .Where(c => c.Users.Any(user => user.Id == userId))
                .ToList();
            return Ok(conversations);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConversation([FromBody] ConversationRequestDto conversationRequestDto)
        {
            var users = _context.Users
                .Where(u => conversationRequestDto.UserIds.Contains(u.Id))
                .ToList();

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Users = users
            };
            await RabbitMQService.CreateExchange("conversation-" + conversation.Id.ToString());

            foreach (var u in users)
            {
                await RabbitMQService.CreateQueue("conversation-" + conversation.Id.ToString() + "-username-" + u.Username, "conversation-" + conversation.Id.ToString());
            }

            _context.Conversations.Add(conversation);
            _context.SaveChanges();

            return Created();
        }

        [HttpPost("{id}/add-chat")]
        public async Task<IActionResult> AddChatToConversation(Guid id, [FromBody] ChatRequestDto chatRequestDto)
        {
            var user = _context.Users.Find(chatRequestDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var chat = chatRequestDto.ToChat(user);
            _context.Chats.Add(chat);

            conversation.Chats.Add(chat);
            _context.Conversations.Update(conversation);

            _context.SaveChanges();

            await RabbitMQService.PublishMessage("conversation-" + conversation.Id.ToString(), JsonSerializer.Serialize(chat));

            return CreatedAtAction(nameof(AddChatToConversation), new { id = chat.Id }, chat);
        }
    }   
}