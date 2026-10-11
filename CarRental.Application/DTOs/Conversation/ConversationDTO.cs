using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.DTOs.Message;

namespace CarRental.Application.DTOs.Conversation
{
    public class ConversationDTO
    {
        public int Id { get; set; }

        public string User1Id { get; set; } = string.Empty;

        public string User2Id { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<MessageDTO> Messages { get; set; }
            = new();
    }
}
