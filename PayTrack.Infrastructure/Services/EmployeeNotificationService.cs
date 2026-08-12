using Microsoft.AspNetCore.SignalR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class EmployeeNotificationService
       : IEmployeeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmployeeNotificationPreferenceRepository _employeeNotificationPreferenceRepository;

        public EmployeeNotificationService(
            IHubContext<NotificationHub> hubContext, IEmployeeNotificationPreferenceRepository employeeNotificationPreferenceRepository)
        {
            _hubContext = hubContext;
            _employeeNotificationPreferenceRepository = employeeNotificationPreferenceRepository;
        }

        public async Task SendToUserAsync(
            string applicationUserId,
            EmployeeNotificationDTO notification)
        {
            var preferences = await _employeeNotificationPreferenceRepository.GetOrCreateAsync(applicationUserId);
            Console.WriteLine("=================================");
            Console.WriteLine($"User: {applicationUserId}");
            Console.WriteLine($"Notification: {notification.Type}");
            Console.WriteLine($"SalaryAdvanceApproved: {preferences.SalaryAdvanceApproved}");
            Console.WriteLine($"SalaryAdvanceRejected: {preferences.SalaryAdvanceRejected}");
            Console.WriteLine($"PayrollGenerated: {preferences.PayrollGenerated}");
            Console.WriteLine($"PayrollReleased: {preferences.PayrollReleased}");
            Console.WriteLine("=================================");
            var shouldSend = notification.Type switch
            {
                EmployeeNotificationType.SalaryAdvanceApproved => preferences.SalaryAdvanceApproved,
                EmployeeNotificationType.SalaryAdvanceRejected => preferences.SalaryAdvanceRejected,
                EmployeeNotificationType.PayrollGenerated => preferences.PayrollGenerated,
                EmployeeNotificationType.PayrollReleased => preferences.PayrollReleased,
                _ => true
            };
            if (!shouldSend)
            {
                return;
            }

            await _hubContext
                .Clients
                .User(applicationUserId)
                .SendAsync(
                    "ReceiveEmployeeNotification",
                    notification);
        }
    }
}


