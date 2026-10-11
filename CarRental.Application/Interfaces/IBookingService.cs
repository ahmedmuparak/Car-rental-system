using CarRental.Application.DTOs.Booking;
using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDTO>> GetAllBookings();
        Task<BookingDTO> GetBookingById(int id);
        Task<BookingDTO> CreateBooking(CreateBookingDTO createBookingDTO, string userId);
        Task<List<BookingDTO>> GetBookingsByUserId(string userId);
        Task<BookingDTO> CancelBooking(int id, string userId);
        Task<BookingDTO> ConfirmBooking(int id);
        Task<BookingDTO> CompleteBooking(int id);
        Task<BookingDTO> UpdatePaymentStatus(int id, PaymentStatus paymentStatus);
    }
}
