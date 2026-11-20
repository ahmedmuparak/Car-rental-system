using System.Net;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Invoices;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CarRental.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly CarRentalDbContext context;
        private readonly IEmailService emailService;
        private readonly ILogger<InvoiceService> logger;

        private const string Currency = "USD";

        public InvoiceService(CarRentalDbContext context,IEmailService emailService, ILogger<InvoiceService> logger)
        {
            this.context = context;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int bookingId)
        {
            var model = await BuildModelAsync(bookingId);
            return new InvoiceDocument(model).ToPdf();
        }

        public async Task SendInvoiceEmailAsync(int bookingId)
        {
            var model = await BuildModelAsync(bookingId);
            var pdf = new InvoiceDocument(model).ToPdf();

            await emailService.SendEmailWithAttachmentAsync(
                model.CustomerEmail,
                $"Your booking invoice {model.InvoiceNumber}",
                BuildEmailBody(model),
                $"Invoice-{model.InvoiceNumber}.pdf",
                pdf);

            logger.LogInformation(
                "Invoice {InvoiceNumber} sent for booking {BookingId}",
                model.InvoiceNumber, bookingId);
        }

        private async Task<InvoiceModel> BuildModelAsync(int bookingId)
        {
            var data = await (
                from b in context.Bookings.AsNoTracking()
                join u in context.Users on b.CustomerId equals u.Id
                where b.Id == bookingId
                select new
                {
                    Booking = b,
                    Customer = u,
                    Car = b.Car,
                    Pickup = b.PickupBranch,
                    Return = b.ReturnBranch
                }
            ).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new NotFoundException("Booking not found");
            }

            if (string.IsNullOrWhiteSpace(data.Customer.Email))
            {
                throw new BadRequestException("Customer has no email address");
            }

            var booking = data.Booking;

            return new InvoiceModel(
                InvoiceNumber: $"INV-{booking.Id:D6}",
                IssuedAt: DateTime.UtcNow,
                CustomerName: data.Customer.FullName,
                CustomerEmail: data.Customer.Email!,
                CustomerPhone: data.Customer.PhoneNumber,
                CarName: $"{data.Car.Brand} {data.Car.Model}",
                CarYear: data.Car.Year,
                CarColor: data.Car.Color,
                PickupBranch: $"{data.Pickup.Name}, {data.Pickup.City}",
                ReturnBranch: $"{data.Return.Name}, {data.Return.City}",
                StartDate: booking.StartDate,
                EndDate: booking.EndDate,
                TotalDays: (int)booking.TotalDays,
                PricePerDay: data.Car.PricePerDay,
                Discount: booking.Discount,
                TotalPrice: booking.TotalPrice,
                BookingStatus: booking.Status.ToString(),
                PaymentStatus: booking.PaymentStatus.ToString(),
                Currency: Currency);
        }

        private static string BuildEmailBody(InvoiceModel m)
        {
            // Names come from user input, so they are HTML-encoded.
            var name = WebUtility.HtmlEncode(m.CustomerName);
            var car = WebUtility.HtmlEncode(m.CarName);

            return $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:auto;color:#222"">
  <h2 style=""color:#1a3d8f"">Booking received</h2>
  <p>Hi {name},</p>
  <p>Thanks for your booking. Your invoice <b>{m.InvoiceNumber}</b> is attached to this email as a PDF.</p>
  <table style=""border-collapse:collapse;width:100%"">
    <tr><td style=""padding:6px 0"">Car</td><td style=""text-align:right"">{car}</td></tr>
    <tr><td style=""padding:6px 0"">Pickup</td><td style=""text-align:right"">{m.StartDate:dd MMM yyyy}</td></tr>
    <tr><td style=""padding:6px 0"">Return</td><td style=""text-align:right"">{m.EndDate:dd MMM yyyy}</td></tr>
    <tr><td style=""padding:6px 0""><b>Total</b></td><td style=""text-align:right""><b>{m.TotalPrice:N2} {m.Currency}</b></td></tr>
  </table>
  <p style=""color:#666"">Payment status: {m.PaymentStatus}. Your booking is {m.BookingStatus} until our team confirms it.</p>
</div>";
        }
    }
}
