using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public class CreateEmployeeOvertimeCommandHandler
       : IRequestHandler<CreateEmployeeOvertimeCommand, Guid>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateEmployeeOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateEmployeeOvertimeCommand request, CancellationToken cancellationToken)
        {
            if (request.EndTime <= request.StartTime)
            {
                throw new InvalidOperationException(
                    "Overtime end time must be later than start time.");
            }

            if (request.Date.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    "Overtime cannot be requested for a past date.");
            }

            var userId =
                _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
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
                    "Employee not found.");
            }

            var hasOverlap =
                await _overtimeRepository.HasOverlapAsync(
                    employee.Id,
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    null);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "The overtime request overlaps with an existing overtime request.");
            }

            var requestedHours =
                (decimal)(
                    request.EndTime -
                    request.StartTime
                ).TotalHours;

            if (requestedHours <= 0)
            {
                throw new InvalidOperationException(
                    "Overtime hours must be greater than zero.");
            }

            var overtime = new OvertimeRequest
            {
                Id = Guid.NewGuid(),

                EmployeeId = employee.Id,

                Date = request.Date.Date,

                StartTime = request.StartTime,

                EndTime = request.EndTime,

                RequestedHours = requestedHours,

                ActualHours = 0,

                OvertimeRate = 0,

                OvertimePay = 0,

                Reason = request.Reason,

                Source = OvertimeSource.Employee,

                /*
                 * Employee created the request,
                 * so the employee has already accepted
                 * his/her own request.
                 */
                EmployeeResponse =
                    OvertimeEmployeeResponse.Accepted,

                Status =
                    OvertimeStatus.PendingAdminApproval,

                CreatedBy = userId,

                CreatedAt = DateTime.UtcNow
            };

            await _overtimeRepository.AddAsync(overtime);


            return overtime.Id;
        }
    }
}
