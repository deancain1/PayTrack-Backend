using Microsoft.AspNetCore.SignalR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class CompanyAdminNotificationService : ICompanyAdminNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public CompanyAdminNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendToUserAsync(string userId, AdminNotificationDTO notification)
        {
            Console.WriteLine($"Sending to user: {userId}");

            await _hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notification);

            Console.WriteLine("SendAsync completed");
        }
    }
}
