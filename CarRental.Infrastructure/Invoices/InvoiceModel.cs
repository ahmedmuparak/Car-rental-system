namespace CarRental.Infrastructure.Invoices
{
    public record InvoiceModel(
        string InvoiceNumber,
        DateTime IssuedAt,
        string CustomerName,
        string CustomerEmail,
        string? CustomerPhone,
        string CarName,
        int CarYear,
        string CarColor,
        string PickupBranch,
        string ReturnBranch,
        DateTime StartDate,
        DateTime EndDate,
        int TotalDays,
        decimal PricePerDay,
        decimal Discount,
        decimal TotalPrice,
        string BookingStatus,
        string PaymentStatus,
        string Currency);
}