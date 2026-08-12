using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Commands
{
    public class UpdateEmployeeNotificationPreferencesCommandHandler : IRequestHandler<UpdateEmployeeNotificationPreferencesCommand, bool>
    {
        private readonly IEmployeeNotificationPreferenceRepository
            _employeeNotificationPreferenceRepository;

        private readonly ICurrentUserService
            _currentUserService;

        public UpdateEmployeeNotificationPreferencesCommandHandler(
            IEmployeeNotificationPreferenceRepository employeeNotificationPreferenceRepository,
            ICurrentUserService currentUserService)
        {
            _employeeNotificationPreferenceRepository = employeeNotificationPreferenceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            UpdateEmployeeNotificationPreferencesCommand request,
            CancellationToken cancellationToken)
        {
        
            var applicationUserId =
                _currentUserService.UserId;

          
            var preferences =
                await _employeeNotificationPreferenceRepository
                    .GetOrCreateAsync(applicationUserId);

         
            preferences.SalaryAdvanceApproved =
                request.SalaryAdvanceApproved;

            preferences.SalaryAdvanceRejected =
                request.SalaryAdvanceRejected;

            preferences.PayrollGenerated =
                request.PayrollGenerated;

            preferences.PayrollReleased =
                request.PayrollReleased;

           
            await _employeeNotificationPreferenceRepository
                .UpdateAsync(preferences);

            return true;
        }
    }
}

