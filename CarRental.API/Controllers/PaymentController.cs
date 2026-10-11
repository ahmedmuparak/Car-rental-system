using CarRental.Application.DTOs.Payment;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [Authorize(Roles =AppRoles.Customer)]
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult>
            CreateCheckoutSession(
                CreateStripePaymentDTO model)
        {
            try
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "User is not authenticated."
                    });
                }


                var result =
                    await paymentService
                        .CreateStripeCheckoutSessionAsync(
                            model,
                            userId);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);

                var json = await reader.ReadToEndAsync();

                var signature =
                    Request.Headers["Stripe-Signature"]
                        .FirstOrDefault();

                if (string.IsNullOrEmpty(signature))
                {
                    return BadRequest(new
                    {
                        message = "Stripe signature is missing."
                    });
                }

                await paymentService.HandleStripeWebhookAsync(
                    json,
                    signature);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}