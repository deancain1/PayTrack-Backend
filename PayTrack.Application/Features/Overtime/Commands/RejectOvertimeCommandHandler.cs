using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public class RejectOvertimeCommandHandler
        : IRequestHandler<RejectOvertimeCommand>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ICurrentUserService _currentUserService;

        public RejectOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RejectOvertimeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId =
                _currentUserService.UserId;

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

            if (overtime.Source !=
                OvertimeSource.Employee)
            {
                throw new InvalidOperationException(
                    "This overtime request was created by the Company Admin.");
            }

            if (overtime.Status !=
                OvertimeStatus.PendingAdminApproval)
            {
                throw new InvalidOperationException(
                    "This overtime request is not awaiting admin approval.");
            }

            if (overtime.Employee.CompanyAdminId !=
                currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to reject this overtime request.");
            }

            overtime.Status =
                OvertimeStatus.Rejected;

            overtime.AdminRemarks =
                request.Remarks;

            _overtimeRepository.Update(overtime);

            await _overtimeRepository.SaveChangesAsync();
        }
    }
}
