using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Notification.Events
{
    public class CompanyAdminNotificationCreatedEventHandler
        : INotificationHandler<CompanyAdminNotificationCreatedEvent>
    {
        private readonly ICompanyAdminNotificationRepository _notificationRepository;
        private readonly ICompanyAdminNotificationService _notificationService;

        public CompanyAdminNotificationCreatedEventHandler(
            ICompanyAdminNotificationRepository notificationRepository,
            ICompanyAdminNotificationService notificationService)
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(
            CompanyAdminNotificationCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            // Save notification to the database
            var entity = new Domain.Entities.CompanyAdminNotification
            {
                Id = notification.NotificationId,
                CompanyAdminId = notification.CompanyAdminId,
                Type = notification.Type,
                Message = notification.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.AddAsync(entity);

            // Send real-time notification via SignalR
            await _notificationService.SendToUserAsync(
                notification.CompanyAdminId,
                new AdminNotificationDTO(
                    entity.Id,
                    entity.Message,
                    entity.Type,
                    entity.CreatedAt,
                    entity.IsRead
                ));
        }
    }
}
