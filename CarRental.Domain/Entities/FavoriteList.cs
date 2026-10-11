using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Entities
{
    public class FavoriteList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
