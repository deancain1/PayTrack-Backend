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
    public class CreateCompanyAdminOvertimeCommandHandler
       : IRequestHandler<CreateCompanyAdminOvertimeCommand, Guid>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateCompanyAdminOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateCompanyAdminOvertimeCommand request, CancellationToken cancellationToken)
        {
            if (request.EndTime <= request.StartTime)
            {
                throw new InvalidOperationException(
                    "Overtime end time must be later than start time.");
            }

            if (request.Date.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    "Overtime cannot be created for a past date.");
            }

            var currentUserId =
                _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var employee =
                await _employeesRepository.GetByIdAsync(
                    request.EmployeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee not found.");
            }

            /*
             * Make sure the employee belongs to
             * the currently logged-in Company Admin.
             */
            if (employee.CompanyAdminId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to create overtime for this employee.");
            }

            var hasOverlap =
                await _overtimeRepository.HasOverlapAsync(
                    request.EmployeeId,
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

                EmployeeId = request.EmployeeId,

                Date = request.Date.Date,

                StartTime = request.StartTime,

                EndTime = request.EndTime,

                RequestedHours = requestedHours,

                ActualHours = 0,

                OvertimeRate = 0,

                OvertimePay = 0,

                Reason = request.Reason,

                Source = OvertimeSource.CompanyAdmin,

                EmployeeResponse =
                    OvertimeEmployeeResponse.Pending,

                Status =
                    OvertimeStatus.PendingEmployeeResponse,

                CreatedBy = currentUserId,

                CreatedAt = DateTime.UtcNow
            };

            await _overtimeRepository.AddAsync(
                overtime);

            return overtime.Id;
        }
    }
}
