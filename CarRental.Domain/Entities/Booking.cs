using CarRental.Domain.Enums;

namespace CarRental.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public int CarId { get; set; }

        public int PickupBranchId { get; set; }

        public int ReturnBranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal TotalDays { get; set; }
        public decimal Discount { get; set; }

        public BookingStatus Status { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public Payment? Payment { get; set; }
        public Car Car { get; set; } = null!;

        public Branch PickupBranch { get; set; } = null!;

        public Branch ReturnBranch { get; set; } = null!;
    }
}