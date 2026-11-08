using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly CarRentalDbContext context;

        public ChatHub(CarRentalDbContext context)
        {
            this.context = context;
        }

        public async Task JoinConversation(int conversationId)
        {
            var userId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User is not authenticated.");
            }

            var isParticipant = await context.Conversations
                .AsNoTracking()
                .AnyAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == userId || c.User2Id == userId));

            if (!isParticipant)
            {
                throw new HubException("You are not part of this conversation.");
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"conversation-{conversationId}");
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"conversation-{conversationId}");
        }
    }
}