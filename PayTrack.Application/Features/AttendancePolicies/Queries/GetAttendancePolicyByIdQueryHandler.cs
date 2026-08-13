using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Queries
{
    public class GetAttendancePolicyByIdQueryHandler : IRequestHandler<GetAttendancePolicyByIdQuery, AttendancePolicyDTO?>
    {
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        public GetAttendancePolicyByIdQueryHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task<AttendancePolicyDTO?> Handle(GetAttendancePolicyByIdQuery request, CancellationToken cancellationToken)
        {
            var policy = await _attendancePolicyRepository.GetByIdAsync(request.Id);

            if (policy == null)
                return null;

            return new AttendancePolicyDTO(
                policy.Id,
                policy.PolicyName,
                policy.IsDefault,
                policy.EnableLateDeduction,
                policy.LateDeductionType,
                policy.DeductionValue);
        }
    }
    
}
