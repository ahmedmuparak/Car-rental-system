using CarRental.Application.DTOs.Payment;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace CarRental.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly CarRentalDbContext context;
        private readonly StripeSettingsDTO stripeSettings;

        public PaymentService(
            CarRentalDbContext context,
            IOptions<StripeSettingsDTO> stripeOptions)
        {
            this.context = context;

            stripeSettings = stripeOptions.Value;

            StripeConfiguration.ApiKey =
                stripeSettings.SecretKey;
        }

        public async Task<StripeCheckoutResponseDTO>
            CreateStripeCheckoutSessionAsync(
                CreateStripePaymentDTO model,
                string userId)
        {

            var booking = await context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.Id == model.BookingId &&
                    b.CustomerId == userId);

            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            if (booking.PaymentStatus == PaymentStatus.Paid)
            {
                throw new Exception(
                    "This booking has already been paid.");
            }

            if (booking.TotalPrice <= 0)
            {
                throw new Exception(
                    "Invalid booking amount.");
            }

            var existingPayment = await context.Payments
                .FirstOrDefaultAsync(p =>
                    p.BookingId == booking.Id &&
                    p.Status == PaymentStatus.Pending);


            if (existingPayment != null &&
                !string.IsNullOrEmpty(
                    existingPayment.StripeSessionId))
            {
                return new StripeCheckoutResponseDTO
                {
                    SessionId =
                        existingPayment.StripeSessionId,

                    CheckoutUrl =
                        existingPayment.StripeCheckoutUrl
                        ?? string.Empty
                };
            }

            var options = new SessionCreateOptions
            {
                Mode = "payment",

                SuccessUrl =
                    stripeSettings.SuccessUrl,

                CancelUrl =
                    stripeSettings.CancelUrl,

                ClientReferenceId =
                    booking.Id.ToString(),

                Metadata = new Dictionary<string, string>
                {
                    {
                        "BookingId",
                        booking.Id.ToString()
                    },

                    {
                        "CustomerId",
                        userId
                    }
                },

                LineItems =
                    new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Quantity = 1,

                            PriceData =
                                new SessionLineItemPriceDataOptions
                                {
                                    Currency = "usd",

                                    UnitAmount =
                                        (long)(
                                            booking.TotalPrice * 100
                                        ),

                                    ProductData =
                                        new SessionLineItemPriceDataProductDataOptions
                                        {
                                            Name =
                                                $"Car Rental Booking #{booking.Id}"
                                        }
                                }
                        }
                    }
            };

            var stripeService =
                new SessionService();

            var session =
                await stripeService.CreateAsync(options);

            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                Method = CarRental.Domain.Enums.PaymentMethod.Online,
                Status = PaymentStatus.Pending,
                StripeSessionId = session.Id,
                StripeCheckoutUrl = session.Url,
                PaymentDate = DateTime.UtcNow
            };

            await context.Payments.AddAsync(payment);

            await context.SaveChangesAsync();

            return new StripeCheckoutResponseDTO
            {
                SessionId = session.Id,

                CheckoutUrl = session.Url
            };
        }

        public async Task HandleStripeWebhookAsync(
    string json,
    string stripeSignature)
        {
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    stripeSettings.WebhookSecret
                );
            }
            catch (Exception)
            {
                throw new Exception("Invalid Stripe webhook signature.");
            }

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session =
                    stripeEvent.Data.Object
                    as Stripe.Checkout.Session;

                if (session == null)
                    return;

                var payment = await context.Payments
                    .Include(p => p.Booking)
                    .FirstOrDefaultAsync(p =>
                        p.StripeSessionId == session.Id);

                if (payment == null)
                    return;

                if (payment.Status == PaymentStatus.Paid)
                    return;

                payment.Status = PaymentStatus.Paid;

                payment.TransactionId =
                    session.PaymentIntentId;

                payment.PaymentDate =
                    DateTime.UtcNow;

                payment.Booking.PaymentStatus =
                    PaymentStatus.Paid;

                await context.SaveChangesAsync();
            }
        }
    }
}