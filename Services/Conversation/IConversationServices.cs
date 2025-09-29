using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Services
{
    public interface IConversationServices
    {
        Task<Conversation> CreateConversation(ConversationRequestDto conversationRequestDto);
        Task<Chat> AddChatToConversation(ChatRequestDto chatRequestDto, User user, Conversation conversation);
    }
}