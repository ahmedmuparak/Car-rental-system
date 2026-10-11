using CarRental.Application.DTOs.Notification;
using CarRental.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(CreateNotificationDTO notificationDTO);
        Task<List<Notification>> GetNotificationsByUserId(string userId);
        Task MarkNotificationAsRead(int notificationId);
        Task<List<Notification>> GetUnreadNotificationsByUserId(string userId);
        Task<int> GetUnreadNotificationCountByUserId(string userId);
    }
}
