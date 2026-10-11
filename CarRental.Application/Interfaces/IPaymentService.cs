using CarRental.Application.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<StripeCheckoutResponseDTO>CreateStripeCheckoutSessionAsync(CreateStripePaymentDTO model,string userId);
        Task HandleStripeWebhookAsync(string json,string stripeSignature);
    }
}
