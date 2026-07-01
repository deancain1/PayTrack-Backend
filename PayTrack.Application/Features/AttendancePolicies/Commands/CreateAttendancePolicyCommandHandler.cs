using MediatR;
using PayTrack.Application.Interfaces;
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
        public CreateAttendancePolicyCommandHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task<Guid> Handle(CreateAttendancePolicyCommand request, CancellationToken cancellationToken)
        {
            // Only one default policy per company
            if (request.IsDefault)
            {
                var currentDefault =
                    await _attendancePolicyRepository.GetDefaultPolicyAsync(request.CompanyAdminId);

                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;

                    await _attendancePolicyRepository.UpdateAsync(currentDefault);
                }
            }

            var policy = new AttendancePolicy
            {
                Id = Guid.NewGuid(),
                CompanyAdminId = request.CompanyAdminId,
                Name = request.Name,
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
