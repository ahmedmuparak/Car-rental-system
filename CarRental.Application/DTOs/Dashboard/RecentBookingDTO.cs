using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Dashboard
{
    public class RecentBookingDTO
    {
        public int BookingId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CarName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }
}
