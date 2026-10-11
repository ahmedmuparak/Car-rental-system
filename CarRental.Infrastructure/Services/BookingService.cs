using CarRental.Application.DTOs.Booking;
using CarRental.Application.DTOs.Notification;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Identity;
using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly CarRentalDbContext context;
        private readonly ICarService carService;
        private readonly INotificationService notificationService;
        private readonly UserManager<ApplicationUser> userManager;

        public BookingService(CarRentalDbContext context
            ,ICarService carService
            ,INotificationService notificationService
            ,UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.carService = carService;
            this.notificationService = notificationService;
            this.userManager = userManager;
        }


        public async Task<List<BookingDTO>> GetAllBookings()
        {
            var bookings = await (
                from booking in context.Bookings
                join user in context.Users
                    on booking.CustomerId equals user.Id
                select new 
                {
                    booking= booking,
                    user= user

                }
            ).ToListAsync();

            return bookings
                .Select(b => MapToDTO(b.booking, b.user))
                .ToList();
        }

       public async Task<BookingDTO> GetBookingById(int id)
        {
            var booking = await (
                from b in context.Bookings
                join user in context.Users
                    on b.CustomerId equals user.Id
                where b.Id == id
                select new
                {
                    booking = b,
                    user = user
                }
            ).FirstOrDefaultAsync();
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            return MapToDTO(booking.booking, booking.user);

        }

        public async Task<BookingDTO> CreateBooking(CreateBookingDTO createBookingDTO , string userId)
        {

            if (createBookingDTO.EndDate <= createBookingDTO.StartDate)
            {
                throw new Exception("End date must be greater than start date");
            }


            var customer = await context.Users
                .FindAsync(userId);

            if (customer == null)
            {
                throw new Exception("User not found");
            }

            var car = await context.Cars
                .FirstOrDefaultAsync(c => c.Id == createBookingDTO.CarId);

            if (car == null)
            {
                throw new Exception("Car not found");
            }

            if (car.Status != CarStatus.Available)
            {
                throw new Exception("Car is not available");
            }

            var pickupBranch = await context.Branches
                .FirstOrDefaultAsync(b => b.Id == createBookingDTO.PickupBranchId);

            if (pickupBranch == null)
            {
                throw new Exception("Pickup branch not found");
            }

            var returnBranch = await context.Branches
                .FirstOrDefaultAsync(b => b.Id == createBookingDTO.ReturnBranchId);

            if (returnBranch == null)
            {
                throw new Exception("Return branch not found");
            }

            var totalDays = (createBookingDTO.EndDate - createBookingDTO.StartDate).Days;

            if (totalDays <= 0)
            {
                throw new Exception("Booking duration must be at least one day");
            }

            var Discount = car.Discount ?? 0;

            var totalPrice =
                (car.PricePerDay * totalDays) - (Discount);

            if (totalPrice < 0)
            {
                totalPrice = 0;
            }


            var booking = new Booking
            {
                CustomerId = userId,

                CarId = createBookingDTO.CarId,

                PickupBranchId = createBookingDTO.PickupBranchId,

                ReturnBranchId = createBookingDTO.ReturnBranchId,

                StartDate = createBookingDTO.StartDate,

                EndDate = createBookingDTO.EndDate,

                TotalDays = totalDays,

                TotalPrice = totalPrice,

                Discount = Discount,

                Status = BookingStatus.Pending,
                
                PaymentStatus = PaymentStatus.Pending
            };

            context.Bookings.Add(booking);



            var admins = await userManager.GetUsersInRoleAsync("SuperAdmin");
            var employees = await userManager.GetUsersInRoleAsync("Employee");

            var recipients = admins
                .Concat(employees)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var user in recipients)
            {
                await notificationService.CreateNotification(
                    new CreateNotificationDTO
                    {
                        UserId = user.Id,
                        Title = "New Booking",
                        Message = $"A customer has created a new booking #{booking.Id}.",
                        Type = NotificationType.Booking
                    });
            }


            await context.SaveChangesAsync();

            return MapToDTO(booking, customer);
        }

        public async Task<List<BookingDTO>> GetBookingsByUserId(string userId)
        {
            var bookings = await context.Bookings
                .Where(b => b.CustomerId == userId)
                .Join(context.Users,
                    booking => booking.CustomerId,
                    user => user.Id,
                    (booking, user) => new { booking, user })
                .Select(b => MapToDTO(b.booking, b.user))
                .ToListAsync();

            return bookings;
        }

        public async Task<BookingDTO> CancelBooking(int id,string userId)
        {
            var booking = await (
                from b in context.Bookings
                join user in context.Users
                    on b.CustomerId equals user.Id
                where b.Id == id
                select new
                {
                    booking = b,
                    user = user
                }
            )
            .FirstOrDefaultAsync();
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            if (booking.booking.CustomerId != userId)
            {
                throw new Exception("You are not allowed to cancel this booking");
            }

            if (booking.booking.Status == BookingStatus.Completed)
            {
                throw new Exception(
                    "Completed booking cannot be cancelled");
            }

            if (booking.booking.Status == BookingStatus.Cancelled)
            {
                throw new Exception(
                    "Booking is already cancelled");
            }

            booking.booking.Status = BookingStatus.Cancelled;
            await carService.UpdateCarStatus(booking.booking.CarId, CarStatus.Available);


            var admins = await userManager.GetUsersInRoleAsync("SuperAdmin");
            var employees = await userManager.GetUsersInRoleAsync("Employee");

            var recipients = admins
                .Concat(employees)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var user in recipients)
            {
                await notificationService.CreateNotification(
                    new CreateNotificationDTO
                    {
                        UserId = user.Id,
                        Title = "Cancelled Booking",
                        Message = $"A customer has cancelled a booking #{booking.booking.Id}.",
                        Type = NotificationType.BookingCancelled
                    });
            }

            await context.SaveChangesAsync();
            return MapToDTO(booking.booking, booking.user);
        }

        public async Task<BookingDTO> ConfirmBooking(int id)
        {
            var booking = await(
                from b in context.Bookings
                join user in context.Users
                    on b.CustomerId equals user.Id
                where b.Id == id
                select new
                {
                    booking = b,
                    user = user
                }
            )
            .FirstOrDefaultAsync();
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == booking.booking.CarId);
            if (car == null)
            {
                throw new Exception("Car not found");
            }
            booking.booking.Status = BookingStatus.Confirmed;
            car.Status = CarStatus.Rented;
            await context.SaveChangesAsync();
            return MapToDTO(booking.booking, booking.user);
        }

        public async Task<BookingDTO> UpdatePaymentStatus(int id,PaymentStatus paymentStatus)
        {
            var booking = await (
                from b in context.Bookings
                join user in context.Users
                    on b.CustomerId equals user.Id
                where b.Id == id
                select new
                {
                    booking = b,
                    user = user
                }
            )
            .FirstOrDefaultAsync();

            if (booking == null)
            { 
                throw new Exception("Booking not found");
            }

            booking.booking.PaymentStatus = paymentStatus;
            await context.SaveChangesAsync();
            return MapToDTO(booking.booking, booking.user);
        }

        public async Task<BookingDTO> CompleteBooking(int id)
        {
            var booking = await context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                throw new Exception(
                    "Only confirmed bookings can be completed");
            }

            var customer = await context.Users
              .FirstOrDefaultAsync(u => u.Id == booking.CustomerId);

            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var car = await context.Cars
    .FirstOrDefaultAsync(c => c.Id == booking.CarId);

            if (car == null)
            {
                throw new Exception("Car not found");
            }

            booking.Status = BookingStatus.Completed;

            car.Status = CarStatus.Available;

            await context.SaveChangesAsync();

            return MapToDTO(booking, customer);

        }
        private static BookingDTO MapToDTO(Booking booking, ApplicationUser user)
        {
            return new BookingDTO
            {
                Id = booking.Id,

                CustomerId = user.Id,
                CustomerName = user.FullName,
                CustomerEmail = user.Email!,
                CustomerPhone = user.PhoneNumber!,

                CarId = booking.CarId,
                PickupBranchId = booking.PickupBranchId,
                ReturnBranchId = booking.ReturnBranchId,

                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalDays = booking.TotalDays,

                TotalPrice = booking.TotalPrice,
                Discount = booking.Discount,

                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus
            };
        }
    }
}
