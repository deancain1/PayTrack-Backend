using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, NotificationDTO notification);
    }
}
