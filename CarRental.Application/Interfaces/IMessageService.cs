using CarRental.Application.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesByConversationId(int conversationId, string userId);
        Task<MessageDTO> SendMessage(int conversationId, string senderId, string content);
        Task MarkMessagesAsRead(int conversationId, string userId);
        Task<IEnumerable<MessageDTO>> GetUnreadMessagesByConversationId(int conversationId, string userId);
        Task<MessageDTO> EditMessage(int messageId, string userId, string newContent);
        Task Delete(int messageId, string userId);
    }
}
