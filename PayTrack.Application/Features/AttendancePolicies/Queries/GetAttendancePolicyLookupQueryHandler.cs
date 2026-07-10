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
       private readonly ICurrentUserService _currentUserService;
        public GetAttendancePolicyLookupQueryHandler(IAttendancePolicyRepository attendancePolicyRepository, ICurrentUserService currentUserService)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
            _currentUserService = currentUserService;
        }
        public async  Task<List<AttendancePolicyLookupDTO>> Handle(GetAttendancePolicyLookupQuery request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId; 
            var policies = await _attendancePolicyRepository.GetAllByCompanyAsync(companyAdminId);

            return policies
                .Select(x => new AttendancePolicyLookupDTO(
                    x.Id,
                    x.PolicyName))
                .ToList();
        }
    }
}
