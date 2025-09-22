using Microsoft.AspNetCore.Mvc;
using Cheting.Data;
using Cheting.Models;
using Cheting.Dtos;
using Cheting.Mappers;


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
            var conversation = _context.Chats.Find(id);
            if (conversation == null)
            {
                return NotFound();
            }
            return Ok(conversation);
        }
        [HttpPost("create")]
        public IActionResult CreateConversation([FromBody] Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetConversationById), new { id = conversation.Id }, conversation);
        }

        [HttpPost("{id}/add-chat")]
        public IActionResult AddChatToConversation(Guid id, [FromBody] ChatRequestDto chatRequestDto)
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

            return CreatedAtAction(nameof(AddChatToConversation), new { id = chat.Id }, chat);
        }
    }   
}