using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public class CreateAttendancePolicyCommandHandler : IRequestHandler<CreateAttendancePolicyCommand, Guid>
    {
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        private readonly ICurrentUserService _currentUserService;
        public CreateAttendancePolicyCommandHandler(IAttendancePolicyRepository attendancePolicyRepository, ICurrentUserService currentUserService)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Guid> Handle(CreateAttendancePolicyCommand request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            // Only one default policy per company
            if (request.IsDefault)
            {
                var currentDefault =
                    await _attendancePolicyRepository.GetDefaultPolicyAsync(companyAdminId);

                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;

                     _attendancePolicyRepository.Update(currentDefault);
                    await _attendancePolicyRepository.SaveChangesAsync();
                }
            }

            
            var policy = new AttendancePolicy
            {
                Id = Guid.NewGuid(),
                CompanyAdminId = companyAdminId,
                PolicyName = request.PolicyName,
                IsDefault = request.IsDefault,
                EnableLateDeduction = request.EnableLateDeduction,
                LateDeductionType = request.LateDeductionType,
                DeductionValue = request.DeductionValue
            };

            await _attendancePolicyRepository.AddAsync(policy);

            return policy.Id;
        }
    }

}
