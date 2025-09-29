using Microsoft.AspNetCore.Mvc;
using Cheting.Data;
using Cheting.Dtos;
using Cheting.Mappers;
using Microsoft.EntityFrameworkCore;
using Cheting.Services;
using System.Threading.Tasks;


namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/conversation")]
    public class ConversationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IConversationServices _conversationServices;

        public ConversationController(ApplicationDBContext context, IConversationServices conversationServices)
        {
            _context = context;
            _conversationServices = conversationServices;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationById([FromRoute] Guid id)
        {
            var conversation = await _context.Conversations.Include(c => c.Users).FirstOrDefaultAsync(c => c.Id == id);
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
            var conversation = await _conversationServices.CreateConversation(conversationRequestDto);

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

            var conversation = await _context.Conversations
                .Where(c => c.Id == id && c.Users.Any(u => u.Id == user.Id))
                .Include(c => c.Chats)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var chat = await _conversationServices.AddChatToConversation(chatRequestDto, user, conversation);

            return CreatedAtAction(nameof(AddChatToConversation), new { id = chat.Id }, chat.ToChatResponseDto());
        }

        [HttpGet("{id}/chats")]
        public async Task<IActionResult> GetAllChatsForConversation([FromRoute] Guid id)
        {
            var conversation = await _context.Conversations.Include(c => c.Chats).ThenInclude(ch => ch.User).Where(c => c.Id==id).FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var chats = conversation.Chats.Select(ch => ch.ToChatResponseDto()).ToList();
            return Ok(chats);
        }
    }
}