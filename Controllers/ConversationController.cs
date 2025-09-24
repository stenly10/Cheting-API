using Microsoft.AspNetCore.Mvc;
using Cheting.Data;
using Cheting.Models;
using Cheting.Dtos;
using Cheting.Mappers;
using Cheting.RabbitMQ;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


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
        public IActionResult GetConversationById([FromRoute] Guid id)
        {
            var conversation = _context.Conversations.Include(c => c.Users).First(c => c.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }
            return Ok(conversation.ToConversationResponseDto());
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllConversations()
        {
            var conversations = await _context.Conversations.Include(c => c.Users).ToListAsync();
            return Ok(conversations.Select(c => c.ToConversationResponseDto()).ToList());
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllConversationsForUser([FromRoute] Guid userId)
        {
            var conversations = await _context.Conversations.Include(c => c.Users)
                .Where(c => c.Users.Any(user => user.Id == userId))
                .ToListAsync();
                
            return Ok(conversations.Select(c => c.ToConversationResponseDto()).ToList());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConversation([FromBody] ConversationRequestDto conversationRequestDto)
        {
            var users = await _context.Users
                .Where(u => conversationRequestDto.UserIds.Contains(u.Id))
                .ToListAsync();

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Users = users
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            
            await RabbitMQService.CreateExchange("conversation-" + conversation.Id.ToString(), "direct");

            foreach (var u in users)
            {
                await RabbitMQService.CreateQueue("conversation-" + conversation.Id.ToString() + "-username-" + u.Username, "conversation-" + conversation.Id.ToString(), $"username-{u.Username}");
            }

            return CreatedAtAction(nameof(GetConversationById), new { id = conversation.Id }, conversation.ToConversationResponseDto());
        }

        [HttpPost("{id}/add-chat")]
        public async Task<IActionResult> AddChatToConversation([FromRoute] Guid id, [FromBody] ChatRequestDto chatRequestDto)
        {
            var user = await _context.Users.FindAsync(chatRequestDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var conversation = await _context.Conversations.Include(c => c.Chats).Include(c => c.Users).FirstAsync(c => c.Id == id);
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var chat = chatRequestDto.ToChat(user, conversation);
            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            foreach (var u in conversation.Users)
                if (u.Id != user.Id)
                    await RabbitMQService.PublishMessage("conversation-" + conversation.Id.ToString(), JsonSerializer.Serialize(chat.ToChatResponseDto()), $"username-{u.Username}");

            return CreatedAtAction(nameof(AddChatToConversation), new { id = chat.Id }, chat.ToChatResponseDto());
        }

        [HttpGet("{id}/chats")]
        public IActionResult GetAllChatsForConversation([FromRoute] Guid id)
        {
            var conversation = _context.Conversations.Find(id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            return Ok(conversation.Chats);
        }
    }
}