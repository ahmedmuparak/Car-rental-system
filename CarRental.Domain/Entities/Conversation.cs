using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }

        public string User1Id { get; set; } = string.Empty;
        public string User2Id { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; }
            = new List<Message>();
    }
}
