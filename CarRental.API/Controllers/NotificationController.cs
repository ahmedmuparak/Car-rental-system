using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetNotificationsByUserId()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var notifications = await notificationService.GetNotificationsByUserId(userId);
                return Ok(notifications);
        }

        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
                await notificationService.MarkNotificationAsRead(notificationId);
                return Ok(new { message = "Notification marked as read" });
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotificationsByUserId()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var notifications = await notificationService.GetUnreadNotificationsByUserId(userId);
                return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCountByUserId()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var count = await notificationService.GetUnreadNotificationCountByUserId(userId);
                return Ok(new { unreadCount = count });
        }
    }
}
