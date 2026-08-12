using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Queries
{
    public class GetCompanyAdminNotificationsQueryHandler : IRequestHandler<GetCompanyAdminNotificationsQuery, List<AdminNotificationDTO>>
    {
        private readonly ICompanyAdminNotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCompanyAdminNotificationsQueryHandler(ICompanyAdminNotificationRepository notificationRepository, ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<AdminNotificationDTO>> Handle(GetCompanyAdminNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByCompanyAdminIdAsync(_currentUserService.UserId);

            return notifications
           .Select(n => new AdminNotificationDTO(
               n.Id,
               n.Message,
               n.Type,
               n.CreatedAt,
               n.IsRead))
           .ToList();
        }
    }
}
