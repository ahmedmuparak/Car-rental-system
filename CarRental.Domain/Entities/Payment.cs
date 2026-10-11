using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Domain.Enums;

namespace CarRental.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public string? StripeSessionId { get; set; }
        public string? StripeCheckoutUrl { get; set; }
        public DateTime PaymentDate { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
