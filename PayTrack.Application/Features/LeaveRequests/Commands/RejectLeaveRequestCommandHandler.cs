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
    public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, bool>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        public RejectLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Remarks))
            {
                throw new ArgumentException(
                    "A reason is required when rejecting a leave request.");
            }

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
                    "Only pending leave requests can be rejected.");
            }

            leaveRequest.Status = LeaveStatus.Rejected;

            leaveRequest.ReviewedBy = companyAdminId;

            leaveRequest.ReviewedAt = DateTime.UtcNow;

            leaveRequest.AdminRemarks =
                request.Remarks.Trim();

            leaveRequest.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();

            return true;
        }
    }
}
