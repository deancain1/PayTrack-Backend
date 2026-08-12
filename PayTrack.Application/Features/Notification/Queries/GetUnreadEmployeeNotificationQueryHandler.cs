using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Queries
{
    public class GetUnreadEmployeeNotificationQueryHandler: IRequestHandler<GetUnreadEmployeeNotificationQuery, int>
    {
        private readonly IEmployeeNotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUnreadEmployeeNotificationQueryHandler(IEmployeeNotificationRepository notificationRepository, ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(GetUnreadEmployeeNotificationQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUnreadCountAsync(
                _currentUserService.UserId);
        }
    }
}
