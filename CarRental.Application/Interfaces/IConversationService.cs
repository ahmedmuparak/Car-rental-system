using CarRental.Application.DTOs.Conversation;
using CarRental.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> CreateConservation(CreateConversationDTO model, string user1Id);
        Task<List<ConversationDTO>> GetMyConversations(string userid);
    }
}
