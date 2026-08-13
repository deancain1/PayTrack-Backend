using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Events
{
    public record class CompanyAdminNotificationCreatedEvent
    (
     Guid NotificationId,
     string CompanyAdminId,
     CompanyAdminNotificationType Type,
     string Message,
     DateTime CreatedAt
    ) : INotification;
}
