using CarRental.Application.DTOs.Message;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CarRental.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly CarRentalDbContext context;
        private readonly IHubContext<ChatHub> hubContext;

        public MessageService(CarRentalDbContext context, IHubContext<ChatHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesByConversationId(
            int conversationId,
            string userId)
        {
            var conversation = await context.Conversations
                .FirstOrDefaultAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == userId || c.User2Id == userId));

            if (conversation == null)
            {
                throw new Exception(
                    "Conversation not found or user is not a participant.");
            }

            await MarkMessagesAsRead(conversationId, userId);

            var messages = await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return messages;
        }


        public async Task<MessageDTO> SendMessage(int conversationId, string senderId, string content)
        {
            var conversation = await context.Conversations
                .FirstOrDefaultAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == senderId || c.User2Id == senderId));
            if (conversation == null)
            {
                throw new Exception("Conversation not found or user is not a participant.");
            }
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            context.Messages.Add(message);
            await context.SaveChangesAsync();
            await hubContext.Clients
.Group($"conversation-{message.ConversationId}")
.SendAsync("ReceiveMessage", message);
            return new MessageDTO
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };


        }

        public async Task MarkMessagesAsRead(int conversationId, string userId)
        {
            var conversationExists = await context.Conversations
                .AnyAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == userId || c.User2Id == userId));

            if (!conversationExists)
            {
                throw new Exception(
                    "Conversation not found or user is not a participant.");
            }

            await context.Messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != userId &&
                    !m.IsRead)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(m => m.IsRead, true));
        }



        public async Task<IEnumerable<MessageDTO>> GetUnreadMessagesByConversationId(int conversationId, string userId)
        {
            var conversationExists = await context.Conversations
                .AnyAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == userId || c.User2Id == userId));
            if (!conversationExists)
            {
                throw new Exception(
                    "Conversation not found or user is not a participant.");
            }
            var unreadMessages = await context.Messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != userId &&
                    !m.IsRead)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();
            return unreadMessages;
        }

        public async Task<MessageDTO> EditMessage(int messageId, string userId, string newContent)
        {
            var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);
            if (message == null)
            {
                throw new Exception("Message not found or user is not the sender.");
            }
            message.Content = newContent;
            await context.SaveChangesAsync();
            return new MessageDTO
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };
        }

        public async Task Delete(int messageId, string userId)
        {
            var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);
            if (message == null)
            {
                throw new Exception("Message not found or user is not the sender.");
            }
            context.Messages.Remove(message);
            await context.SaveChangesAsync();
        }
    }
}
