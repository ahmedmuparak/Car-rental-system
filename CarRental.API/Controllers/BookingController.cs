using CarRental.Application.DTOs.Booking;
using CarRental.Application.Interfaces;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService bookingService;
        private readonly IInvoiceService invoiceService;
        private readonly ILogger<BookingController> logger;

        public BookingController(IBookingService bookingService, IInvoiceService invoiceService, ILogger<BookingController> logger)
        {
            this.bookingService = bookingService;
            this.invoiceService = invoiceService;
            this.logger = logger;
        }


        [Authorize(Roles = "SuperAdmin,Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
                var bookings = await bookingService.GetAllBookings();
                return Ok(bookings);
        }


        [Authorize(Roles = "SuperAdmin,Admin,Employee,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    message = "User is not authenticated"
                });
            }

            var isStaff = User.IsInRole(AppRoles.SuperAdmin) || User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.Employee);

            var booking = await bookingService.GetBookingById(id,userId, isStaff);
                return Ok(booking);
        }

        [Authorize(Roles = "Customer,Employee")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDTO createBookingDTO)
        {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                var booking = await bookingService.CreateBooking(createBookingDTO, userId);

            try
            {
                await invoiceService.SendInvoiceEmailAsync(booking.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Could not send the invoice email for booking {BookingId}", booking.Id);
            }

            return Ok(booking);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetBookingsByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    message = "User is not authenticated"
                });
            }
            var bookings = await bookingService.GetBookingsByUserId(userId);
            return Ok(bookings);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Employee,Customer")]
        [HttpPatch("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                var booking = await bookingService.CancelBooking(id,userId);
                return Ok(booking);
        }


        [Authorize(Roles = "SuperAdmin,Admin,Employee")]
        [HttpPatch("confirm/{id}")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
                var booking = await bookingService.ConfirmBooking(id);
                return Ok(booking);
        }


        [Authorize(Roles = "SuperAdmin,Admin,Employee")]
        [HttpPatch("complete/{id}")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
                var booking = await bookingService.CompleteBooking(id);
                return Ok(booking);
        }


        [Authorize(Roles = "SuperAdmin,Admin,Employee")]
        [HttpPatch("payment/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, PaymentStatus paymentStatus)
        {
                var booking = await bookingService.UpdatePaymentStatus(id, paymentStatus);
                return Ok(booking);
        }


    }
}
