using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Events
{
    public record NotificationCreatedEvent(
      Guid NotificationId,
      string CompanyAdminId,
      NotificationType Type,
      string Message,
      DateTime CreatedAt
  ) : INotification;
}
