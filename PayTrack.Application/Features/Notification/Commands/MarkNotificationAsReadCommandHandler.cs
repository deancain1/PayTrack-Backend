using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Commands
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly ICompanyAdminNotificationRepository _notificationRepository;

        public MarkNotificationAsReadCommandHandler(ICompanyAdminNotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAsReadAsync(request.NotificationId);
        }
    }
}
