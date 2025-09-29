using System.Text.Json;
using Cheting.Data;
using Cheting.Dtos;
using Cheting.Mappers;
using Cheting.Models;
using Cheting.RabbitMQ;

namespace Cheting.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly ApplicationDBContext _context;

        public NotificationServices(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ConversationNotificationDto>> GetNotificationsForUser(User user, List<Conversation> conversations)
        {
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

            return messages;
        }

        public async void NotifyUser(User user, Conversation conversation, Chat chat)
        {
            await RabbitMQService.PublishMessage("conversation-" + conversation.Id.ToString(), JsonSerializer.Serialize(chat.ToChatResponseDto()), $"username-{user.Username}");
        }
    }
}