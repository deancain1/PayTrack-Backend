using Microsoft.AspNetCore.SignalR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendToUserAsync(string userId, NotificationDTO notification)
        {
            Console.WriteLine($"Sending to user: {userId}");

            await _hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notification);

            Console.WriteLine("SendAsync completed");
        }
    }
}
