using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.DTOs.Conversation;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly CarRentalDbContext context;

        public ConversationService(CarRentalDbContext context)
        {
            this.context = context;
        }



        public async Task<Conversation> CreateConservation(CreateConversationDTO model,string user1Id)
        {
            var user2Exists = await context.Users
                .AnyAsync(u => u.Id == model.UserId);

            if (!user2Exists)
            {
                throw new Exception("User not found");
            }

            if (user1Id == model.UserId)
            {
                throw new Exception(
                    "You cannot create a conversation with yourself.");
            }

            var conversationExists = await context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1Id && c.User2Id == model.UserId) ||
                    (c.User1Id == model.UserId && c.User2Id == user1Id));

            if (conversationExists !=null)
            {
                return conversationExists;
            }

            var conversation = new Conversation
            {
                User1Id = user1Id,
                User2Id = model.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await context.Conversations.AddAsync(conversation);

            await context.SaveChangesAsync();

            return conversation;
        }

        public async Task<List<ConversationDTO>> GetMyConversations(string userid)
        {
            var conversations = await context.Conversations
                .Where(c => c.User1Id == userid || c.User2Id == userid)
                .Select(c => new ConversationDTO
                {
                    Id = c.Id,
                    User1Id = c.User1Id,
                    User2Id = c.User2Id,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
            return conversations;
        }
    }
}
