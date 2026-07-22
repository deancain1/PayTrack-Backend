using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Notification.Events
{
    public class NotificationCreatedEventHandler
        : INotificationHandler<NotificationCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;

        public NotificationCreatedEventHandler(
            INotificationRepository notificationRepository,
            INotificationService notificationService)
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(
            NotificationCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            // Save notification to the database
            var entity = new Domain.Entities.Notification
            {
                Id = notification.NotificationId,
                CompanyAdminId = notification.CompanyAdminId,
                Type = notification.Type,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = false
            };

            await _notificationRepository.AddAsync(entity);

            // Send real-time notification via SignalR
            await _notificationService.SendToUserAsync(
                notification.CompanyAdminId,
                new NotificationDTO(
                    entity.Id,
                    entity.Message,
                    entity.Type,
                    entity.CreatedAt,
                    entity.IsRead
                ));
        }
    }
}