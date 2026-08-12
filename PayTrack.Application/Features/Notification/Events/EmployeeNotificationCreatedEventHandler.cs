using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Events
{
    public class EmployeeNotificationCreatedEventHandler
        : INotificationHandler<EmployeeNotificationCreatedEvent>
    {
        private readonly IEmployeeNotificationRepository _notificationRepository;
        private readonly IEmployeeNotificationService _notificationService;
        private readonly IEmployeeNotificationPreferenceRepository _employeeNotificationPreferenceRepository;

        public EmployeeNotificationCreatedEventHandler(
            IEmployeeNotificationRepository notificationRepository,
            IEmployeeNotificationService notificationService,
            IEmployeeNotificationPreferenceRepository employeeNotificationPreferenceRepository)
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
            _employeeNotificationPreferenceRepository = employeeNotificationPreferenceRepository;
        }

        public async Task Handle(
            EmployeeNotificationCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            var preferences = await _employeeNotificationPreferenceRepository.GetOrCreateAsync(notification.ApplicationUserId);

            var shouldSend = notification.Type switch
            {
                EmployeeNotificationType.SalaryAdvanceApproved =>
                    preferences.SalaryAdvanceApproved,

                EmployeeNotificationType.SalaryAdvanceRejected =>
                    preferences.SalaryAdvanceRejected,

                EmployeeNotificationType.PayrollGenerated =>
                    preferences.PayrollGenerated,

                EmployeeNotificationType.PayrollReleased =>
                    preferences.PayrollReleased,

                _ => true
            };

            if (!shouldSend)
            {
                return;
            }


            var entity = new EmployeeNotification
            {
                Id = notification.NotificationId,
                ApplicationUserId = notification.ApplicationUserId,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.AddAsync(entity);

            await _notificationService.SendToUserAsync(
                notification.ApplicationUserId,
                new EmployeeNotificationDTO(
                    entity.Id,
                    entity.Type,
                    entity.Title,
                    entity.Message,
                    entity.IsRead,
                    entity.CreatedAt
                ));
        }
    }
}
