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
    public class GetAttendancePolicyLookupQueryHandler : IRequestHandler<GetAttendancePolicyLookupQuery, List<AttendancePolicyLookupDTO>>
    {
       private readonly IAttendancePolicyRepository _attendancePolicyRepository; 
        public GetAttendancePolicyLookupQueryHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async  Task<List<AttendancePolicyLookupDTO>> Handle(GetAttendancePolicyLookupQuery request, CancellationToken cancellationToken)
        {
            var policies = await _attendancePolicyRepository
               .GetAllByCompanyAsync(request.CompanyAdminId);

            return policies
                .Select(x => new AttendancePolicyLookupDTO(
                    x.Id,
                    x.Name))
                .ToList();
        }
    }
}
