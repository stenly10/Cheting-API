using Cheting.Data;
using Cheting.Dtos;
using Cheting.Mappers;
using Cheting.Models;
using Cheting.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cheting.Services
{
    public class ConversationServices : IConversationServices
    {
        private readonly ApplicationDBContext _context;
        private readonly INotificationServices _notificationServices;

        public ConversationServices(ApplicationDBContext context, INotificationServices notificationServices)
        {
            _context = context;
            _notificationServices = notificationServices;
        }

        public async Task<Conversation> CreateConversation(ConversationRequestDto conversationRequestDto)
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

            return conversation;
        }

        public async Task<Chat> AddChatToConversation(ChatRequestDto chatRequestDto, User user, Conversation conversation)
        {
            var chat = chatRequestDto.ToChat(user, conversation);
            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            foreach (var u in conversation.Users)
                if (u.Id != user.Id)
                    _notificationServices.NotifyUser(u, conversation, chat);
                    
            return chat;
        }
    }
}