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
    public class GetEmployeeNotificationPreferencesQueryHandler : IRequestHandler<GetEmployeeNotificationPreferencesQuery, EmployeeNotificationPreferenceDTO>
    {
        private readonly IEmployeeNotificationPreferenceRepository _employeeNotificationPreferenceRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetEmployeeNotificationPreferencesQueryHandler(IEmployeeNotificationPreferenceRepository employeeNotificationPreferenceRepository, ICurrentUserService currentUserService)
        {
            _employeeNotificationPreferenceRepository = employeeNotificationPreferenceRepository;
            _currentUserService = currentUserService;
        }
        public async Task<EmployeeNotificationPreferenceDTO> Handle(GetEmployeeNotificationPreferencesQuery request, CancellationToken cancellationToken)
        {
           var preference = await _employeeNotificationPreferenceRepository.GetOrCreateAsync(_currentUserService.UserId);
          
            return new EmployeeNotificationPreferenceDTO(
               preference.SalaryAdvanceApproved,
               preference.SalaryAdvanceRejected,
               preference.PayrollGenerated,
               preference.PayrollReleased
           );
        }
    }
}
