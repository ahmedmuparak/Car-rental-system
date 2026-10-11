using CarRental.Application.DTOs.Notification;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class NotificationService: INotificationService
    {
        private readonly CarRentalDbContext context;

        public NotificationService(CarRentalDbContext context)
        {
            this.context = context;
        }

        public Task CreateNotification (CreateNotificationDTO notificationDTO)
        {
            var notification = new Notification
            {
                UserId = notificationDTO.UserId,
                Message = notificationDTO.Message,
                CreatedAt = DateTime.UtcNow,
                Title = notificationDTO.Title,
                IsRead = false,
                Type = notificationDTO.Type
            };
            context.Notifications.Add(notification);
            return context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByUserId(string userId)
        {
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications;
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }
            
            notification.IsRead = true;
            await context.SaveChangesAsync();
            
        }

        public Task<List<Notification>> GetUnreadNotificationsByUserId(string userId)
        {
            var notifications = context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        public Task<int> GetUnreadNotificationCountByUserId(string userId)
        {
            var count = context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
            return count;
        }
    }
}
