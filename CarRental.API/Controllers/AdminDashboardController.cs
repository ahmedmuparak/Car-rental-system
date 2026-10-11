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
            try
            {
                var result = await dashboardService.GetOverviewAsync();
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

        [HttpGet("users-statistics")]
        public async Task<IActionResult> GetUsersStatistics()
        {
            try
            {
                var result = await dashboardService.GetUsersStatisticsAsync();
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

        [HttpGet("cars-statistics")]
        public async Task<IActionResult> GetCarsStatistics()
        {
            try
            {
                var result = await dashboardService.GetCarsStatisticsAsync();
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

        [HttpGet("bookings-statistics")]
        public async Task<IActionResult> GetBookingsStatistics()
        {
            try
            {
                var result = await dashboardService.GetBookingsStatisticsAsync();
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

        [HttpGet("payments-statistics")]
        public async Task<IActionResult> GetPaymentsStatistics()
        {
            try
            {
                var result = await dashboardService.GetPaymentsStatisticsAsync();
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

        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
            try
            {
                var result = await dashboardService.GetRecentBookingsAsync();
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


    }
}
