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
    public class RespondToOvertimeCommandHandler
       : IRequestHandler<RespondToOvertimeCommand>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public RespondToOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RespondToOvertimeCommand request, CancellationToken cancellationToken)
        {
            var userId =
                _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
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

            if (overtime.Status !=
                OvertimeStatus.PendingEmployeeResponse)
            {
                throw new InvalidOperationException(
                    "This overtime request is no longer awaiting an employee response.");
            }

            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(
                        userId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee not found.");
            }

            /*
             * Make sure the current employee is the
             * employee who received the OT request.
             */
            if (overtime.EmployeeId != employee.Id)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to respond to this overtime request.");
            }

            if (request.Accept)
            {
                overtime.EmployeeResponse =
                    OvertimeEmployeeResponse.Accepted;

                overtime.Status =
                    OvertimeStatus.Accepted;
            }
            else
            {
                overtime.EmployeeResponse =
                    OvertimeEmployeeResponse.Rejected;

                overtime.Status =
                    OvertimeStatus.Rejected;
            }

            overtime.EmployeeRemarks =
                request.Remarks;

            overtime.RespondedAt =
                DateTime.UtcNow;

            _overtimeRepository.Update(overtime);
            await _overtimeRepository.SaveChangesAsync();
        }
    }
}
