using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Services
{
    public interface INotificationServices
    {
        Task<List<ConversationNotificationDto>> GetNotificationsForUser(User user, List<Conversation> conversations);
        void NotifyUser(User user, Conversation conversation, Chat chat);
    }
}