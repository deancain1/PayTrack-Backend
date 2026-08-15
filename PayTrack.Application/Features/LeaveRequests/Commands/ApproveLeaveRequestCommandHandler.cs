using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, bool>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        public ApproveLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            if (companyAdminId == string.Empty)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var leaveRequest =
                await _leaveRequestRepository.GetByIdAsync(
                    request.LeaveRequestId);

            if (leaveRequest == null)
            {
                throw new KeyNotFoundException(
                    "Leave request not found.");
            }

            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending leave requests can be approved.");
            }

            leaveRequest.Status = LeaveStatus.Approved;

            leaveRequest.ReviewedBy = companyAdminId;

            leaveRequest.ReviewedAt = DateTime.UtcNow;

            leaveRequest.AdminRemarks =
                string.IsNullOrWhiteSpace(request.Remarks)
                    ? null
                    : request.Remarks.Trim();

            leaveRequest.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();

            return true;
        }
    }
}
