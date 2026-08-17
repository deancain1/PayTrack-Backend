using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestSummaryQueryHandler : IRequestHandler<GetLeaveRequestSummaryQuery, LeaveRequestSummaryDTO>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetLeaveRequestSummaryQueryHandler(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
        }
        public async Task<LeaveRequestSummaryDTO> Handle(GetLeaveRequestSummaryQuery request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;
            var counts = await _leaveRequestRepository.GetLeaveRequestCountsAsync(companyAdminId);

            return new LeaveRequestSummaryDTO
            (
               counts.Total,
               counts.Approved,
               counts.Rejected
            );
        }
    }
}
