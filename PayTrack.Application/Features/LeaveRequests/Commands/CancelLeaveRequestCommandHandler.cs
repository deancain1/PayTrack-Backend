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
    public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, bool>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeesRepository _employeesRepository;
        public CancelLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService, IEmployeesRepository employeesRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
        }
        public async Task<bool> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == string.Empty)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(
                        userId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee record not found.");
            }

            var leaveRequest =
                await _leaveRequestRepository.GetByIdAsync(
                    request.LeaveRequestId);

            if (leaveRequest == null)
            {
                throw new KeyNotFoundException(
                    "Leave request not found.");
            }

            if (leaveRequest.EmployeeId != employee.Id)
            {
                throw new UnauthorizedAccessException(
                    "You cannot cancel this leave request.");
            }

            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending leave requests can be cancelled.");
            }

            leaveRequest.Status = LeaveStatus.Cancelled;

            leaveRequest.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();

            return true;
        }
    }
}
