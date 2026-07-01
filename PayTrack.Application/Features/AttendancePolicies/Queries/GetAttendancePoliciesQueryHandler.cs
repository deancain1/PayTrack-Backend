using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Queries
{
    public class GetAttendancePoliciesQueryHandler : IRequestHandler<GetAttendancePoliciesQuery, List<AttendancePolicyDTO>>
    {
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        public GetAttendancePoliciesQueryHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task<List<AttendancePolicyDTO>> Handle(GetAttendancePoliciesQuery request, CancellationToken cancellationToken)
        {
            var policies = await _attendancePolicyRepository
               .GetAllByCompanyAsync(request.CompanyAdminId);

            return policies
                .Select(x => new AttendancePolicyDTO(
                    x.Id,
                    x.Name,
                    x.IsDefault,
                    x.EnableLateDeduction,
                    x.LateDeductionType,
                    x.DeductionValue))
                .ToList();
        }
    }
}
