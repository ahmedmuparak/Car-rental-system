using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using CarRental.Application.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;
using CarRental.Domain.Enums;

namespace CarRental.Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly CarRentalDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICacheService cacheService;

        public AdminDashboardService(CarRentalDbContext context
            ,UserManager<ApplicationUser> userManager
            ,ICacheService cacheService)
        {
            this.context = context;
            this.userManager = userManager;
            this.cacheService = cacheService;
        }

        public async Task<DashboardOverviewDTO> GetOverviewAsync()
        {
            const string cacheKey = "DashboardOverview";
            var cachedOverview = await cacheService.GetAsync<DashboardOverviewDTO>(cacheKey);
            if (cachedOverview is not null)
            {
                return cachedOverview;
            }
            var totalUsers = await userManager.Users.CountAsync();
            var totalCars = await context.Cars.CountAsync();
            var totalBranches = await context.Branches.CountAsync();
            var totalBookings = await context.Bookings.CountAsync();
            var totalRevenue = await context.Payments.Where(p => p.Status == PaymentStatus.Paid).SumAsync(p => p.Amount);
            var result = new DashboardOverviewDTO
            {
                TotalUsers = totalUsers,
                TotalCars = totalCars,
                TotalBranches = totalBranches,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue
            };

            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<UsersStatisticsDTO> GetUsersStatisticsAsync()
        {
            var users = await userManager.Users.ToListAsync();

            int customers = 0;
            int carOwners = 0;
            int drivers = 0;
            int employees = 0;
            int admins = 0;

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains(AppRoles.Customer))
                    customers++;
                if (roles.Contains(AppRoles.CarOwner))
                    carOwners++;
                if (roles.Contains(AppRoles.Driver))
                    drivers++;
                if (roles.Contains(AppRoles.Employee))
                    employees++;
                if (roles.Contains(AppRoles.Admin))
                    admins++;
            }
            return new UsersStatisticsDTO
            {
                TotalUsers = users.Count,
                TotalCustomers = customers,
                TotalCarOwners = carOwners,
                TotalDrivers = drivers,
                TotalEmployees = employees,
                TotalAdmins = admins
            };
        }

        public async Task<CarsStatisticsDTO> GetCarsStatisticsAsync()
        {
            const string cacheKey = "CarsStatistics";

            var chachedCarstatistics = await cacheService.GetAsync<CarsStatisticsDTO>(cacheKey);
            if (chachedCarstatistics is not null)
            {
                return chachedCarstatistics;
            }
            var totalCars = await context.Cars.CountAsync();
            var availableCars = await context.Cars.CountAsync(c => c.Status == CarStatus.Available);
            var rentedCars = await context.Cars.CountAsync(c => c.Status == CarStatus.Rented);
            var result = new CarsStatisticsDTO
            {
                TotalCars = totalCars,
                AvailableCars = availableCars,
                RentedCars = rentedCars
            };
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<BookingsStatisticsDTO> GetBookingsStatisticsAsync()
        {
            var totalBookings = await context.Bookings.CountAsync();
            var pending =
    await context.Bookings.CountAsync(b =>
        b.Status == BookingStatus.Pending);

            var confirmed =
                await context.Bookings.CountAsync(b =>
                    b.Status == BookingStatus.Confirmed);

            var active =
                await context.Bookings.CountAsync(b =>
                    b.Status == BookingStatus.Active);

            var completed =
                await context.Bookings.CountAsync(b =>
                    b.Status == BookingStatus.Completed);

            var cancelled =
                await context.Bookings.CountAsync(b =>
                    b.Status == BookingStatus.Cancelled);

            return new BookingsStatisticsDTO
            {
                TotalBookings = totalBookings,
                PendingBookings = pending,
                ConfirmedBookings = confirmed,
                ActiveBookings = active,
                CompletedBookings = completed,
                CancelledBookings = cancelled
            };
        }

        public async Task<PaymentsStatisticsDTO> GetPaymentsStatisticsAsync()
        {
            var totalPayments =
                await context.Payments.CountAsync();

            var paid =
                await context.Payments.CountAsync(p =>
                    p.Status == PaymentStatus.Paid);

            var pending =
                await context.Payments.CountAsync(p =>
                    p.Status == PaymentStatus.Pending);

            var failed =
                await context.Payments.CountAsync(p =>
                    p.Status == PaymentStatus.Failed);

            var refunded =
                await context.Payments.CountAsync(p =>
                    p.Status == PaymentStatus.Refunded);

            var revenue =
                await context.Payments
                    .Where(p => p.Status == PaymentStatus.Paid)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0;

            return new PaymentsStatisticsDTO
            {
                TotalPayments = totalPayments,
                PaidPayments = paid,
                PendingPayments = pending,
                FailedPayments = failed,
                RefundedPayments = refunded,
                TotalRevenue = revenue
            };
        }


        public async Task<List<RecentBookingDTO>> GetRecentBookingsAsync()
        {
            var bookings = await context.Bookings
                .Include(b => b.Car)
                .OrderByDescending(b => b.Id)
                .Take(10)
                .ToListAsync();

            var CustomerName = await userManager.Users
                .Where(u => bookings.Select(b => b.CustomerId).Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName);

            return bookings
                .Select(b => new RecentBookingDTO
                {
                    BookingId = b.Id,

                    CustomerName = CustomerName[b.CustomerId],
                    CarName =
                        $"{b.Car.Brand} {b.Car.Model}",

                    StartDate = b.StartDate,

                    EndDate = b.EndDate,

                    TotalPrice = b.TotalPrice,

                    Status = b.Status,

                    PaymentStatus = b.PaymentStatus
                })
                .ToList();
        }
    }
}
