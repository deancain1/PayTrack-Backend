using AutoMapper;
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
    public class GetEmployeeNotificationsQueryHandler : IRequestHandler<GetEmployeeNotificationsQuery, List<EmployeeNotificationDTO>>
    {
        private readonly IEmployeeNotificationRepository _employeeNotificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetEmployeeNotificationsQueryHandler(
            IEmployeeNotificationRepository employeeNotificationRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _employeeNotificationRepository = employeeNotificationRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<List<EmployeeNotificationDTO>> Handle(GetEmployeeNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var notifications =
                await _employeeNotificationRepository.GetByUserIdAsync(userId);

            return notifications
                .Select(notification => new EmployeeNotificationDTO(
                    notification.Id,
                    notification.Type,
                    notification.Title,
                    notification.Message,
                    notification.IsRead,
                    notification.CreatedAt
                ))
                .ToList();
        }
    }
}
