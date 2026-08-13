using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public class UpdateAttendancePolicyCommandHandler : IRequestHandler<UpdateAttendancePolicyCommand>
    {
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        public UpdateAttendancePolicyCommandHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task Handle(UpdateAttendancePolicyCommand request, CancellationToken cancellationToken)
        {
            var policy =
                  await _attendancePolicyRepository.GetByIdAsync(request.Id);

            if (policy == null)
                throw new Exception("Attendance policy not found.");

            // Make this the default policy
            if (request.IsDefault)
            {
                var currentDefault =
                    await _attendancePolicyRepository.GetDefaultPolicyAsync(request.CompanyAdminId);

                if (currentDefault != null &&
                    currentDefault.Id != policy.Id)
                {
                    currentDefault.IsDefault = false;

                     _attendancePolicyRepository.Update(currentDefault);
                    await _attendancePolicyRepository.SaveChangesAsync();
                }
            }

            policy.PolicyName = request.PolicyName;
            policy.IsDefault = request.IsDefault;
            policy.EnableLateDeduction = request.EnableLateDeduction;
            policy.LateDeductionType = request.LateDeductionType;
            policy.DeductionValue = request.DeductionValue;

             _attendancePolicyRepository.Update(policy);
            await _attendancePolicyRepository.SaveChangesAsync();
        }
    }
    
}
