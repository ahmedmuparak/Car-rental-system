using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
                var result = await dashboardService.GetOverviewAsync();
                return Ok(result);
        }

        [HttpGet("users-statistics")]
        public async Task<IActionResult> GetUsersStatistics()
        {
                var result = await dashboardService.GetUsersStatisticsAsync();
                return Ok(result);
        }

        [HttpGet("cars-statistics")]
        public async Task<IActionResult> GetCarsStatistics()
        {
                var result = await dashboardService.GetCarsStatisticsAsync();
                return Ok(result);
        }

        [HttpGet("bookings-statistics")]
        public async Task<IActionResult> GetBookingsStatistics()
        {
                var result = await dashboardService.GetBookingsStatisticsAsync();
                return Ok(result);
        }

        [HttpGet("payments-statistics")]
        public async Task<IActionResult> GetPaymentsStatistics()
        {
                var result = await dashboardService.GetPaymentsStatisticsAsync();
                return Ok(result);
        }

        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
                var result = await dashboardService.GetRecentBookingsAsync();
                return Ok(result);
        }


    }
}
