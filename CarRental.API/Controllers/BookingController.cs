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
        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }


        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.Employee)]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
                var bookings = await bookingService.GetAllBookings();
                return Ok(bookings);
        }

        [Authorize(Roles = AppRoles.Employee)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles = AppRoles.Customer)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
                var booking = await bookingService.GetBookingById(id);
                return Ok(booking);
        }

        [Authorize(Roles = AppRoles.Customer)]
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

        [Authorize(Roles = AppRoles.Employee)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles = AppRoles.Customer)]
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


        [Authorize(Roles = AppRoles.Employee)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPatch("confirm/{id}")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
                var booking = await bookingService.ConfirmBooking(id);
                return Ok(booking);
        }


        [Authorize(Roles = AppRoles.Employee)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPatch("complete/{id}")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
                var booking = await bookingService.CompleteBooking(id);
                return Ok(booking);
        }


        [Authorize(Roles = AppRoles.Employee)]
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPatch("payment/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, PaymentStatus paymentStatus)
        {
                var booking = await bookingService.UpdatePaymentStatus(id, paymentStatus);
                return Ok(booking);
        }


    }
}
