using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public class ApproveOvertimeCommandHandler
        : IRequestHandler<ApproveOvertimeCommand>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ICurrentUserService _currentUserService;

        public ApproveOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ApproveOvertimeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var overtime =
                await _overtimeRepository.GetByIdWithApplicationUserAsync(
                    request.Id);

            if (overtime == null)
            {
                throw new KeyNotFoundException(
                    "Overtime request not found.");
            }

          
            if (overtime.Source != OvertimeSource.Employee)
            {
                throw new InvalidOperationException(
                    "This overtime request was created by the Company Admin and does not require admin approval.");
            }

            if (overtime.Status !=
                OvertimeStatus.PendingAdminApproval)
            {
                throw new InvalidOperationException(
                    "This overtime request is not awaiting admin approval.");
            }

          
            if (overtime.Employee.CompanyAdminId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to approve this overtime request.");
            }

            overtime.Status =
                OvertimeStatus.Accepted;

            overtime.AdminRemarks =
                "Overtime request approved.";

            _overtimeRepository.Update(overtime);

            await _overtimeRepository.SaveChangesAsync();
        }
    }
}