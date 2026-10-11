using CarRental.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<DashboardOverviewDTO> GetOverviewAsync();

        Task<UsersStatisticsDTO> GetUsersStatisticsAsync();

        Task<CarsStatisticsDTO> GetCarsStatisticsAsync();

        Task<BookingsStatisticsDTO> GetBookingsStatisticsAsync();

        Task<PaymentsStatisticsDTO> GetPaymentsStatisticsAsync();

        Task<List<RecentBookingDTO>> GetRecentBookingsAsync();
    }
}
