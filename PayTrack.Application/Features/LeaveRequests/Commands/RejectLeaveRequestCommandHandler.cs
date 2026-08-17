using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public class RejectLeaveRequestCommandHandler
        : IRequestHandler<RejectLeaveRequestCommand, bool>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RejectLeaveRequestCommandHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<bool> Handle(
            RejectLeaveRequestCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Remarks))
            {
                throw new ArgumentException(
                    "A reason is required when rejecting a leave request.");
            }

            var companyAdminId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(companyAdminId))
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

           
            var employee =
                await _employeesRepository.GetByIdAsync(
                    leaveRequest.EmployeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee associated with this leave request was not found.");
            }

            //for employee applicationUserId
            var applicationUser =
                await _userManager.FindByIdAsync(
                    employee.ApplicationUserId);

            if (applicationUser == null)
            {
                throw new KeyNotFoundException(
                    "Application user associated with this employee was not found.");
            }
            //for companyAdmin applicationUserId
            var companyAdmin =
               await _userManager.FindByIdAsync(companyAdminId);

            if (companyAdmin == null)
            {
                throw new KeyNotFoundException(
                    "Company admin associated with this leave request was not found.");
            }

            leaveRequest.Status = LeaveStatus.Rejected;
            leaveRequest.ReviewedBy = companyAdmin.FullName;
            leaveRequest.CompanyAdminId = companyAdminId;
            leaveRequest.ReviewedAt = DateTime.UtcNow;
            leaveRequest.AdminRemarks =
                request.Remarks.Trim();
            leaveRequest.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(leaveRequest);

            await _leaveRequestRepository.SaveChangesAsync();

            await _emailService.SendLeaveRejectedEmailAsync(
                applicationUser.Email!,
                applicationUser.FullName,
                leaveRequest.StartDate,
                leaveRequest.EndDate,
                request.Remarks.Trim());

            return true;
        }
    }
}