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
    public class GetCompanyAdminUnreadNotificationQueryHandler : IRequestHandler<GetCompanyAdminUnreadNotificationQuery, int>
    {
        private readonly ICompanyAdminNotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetCompanyAdminUnreadNotificationQueryHandler(ICompanyAdminNotificationRepository notificationRepository, ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(GetCompanyAdminUnreadNotificationQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUnreadCountAsync(_currentUserService.UserId);
        }
    }
}
