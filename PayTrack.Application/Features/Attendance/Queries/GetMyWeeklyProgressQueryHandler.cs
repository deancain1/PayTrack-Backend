using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetMyWeeklyProgressQueryHandler : IRequestHandler<GetMyWeeklyProgressQuery, WeeklyProgressDTO>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetMyWeeklyProgressQueryHandler(IAttendanceRepository attendanceRepository, IEmployeesRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;   
        }
        public async Task<WeeklyProgressDTO> Handle(GetMyWeeklyProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var employee =
                await _employeeRepository.GetCurrentEmployeeByApplicationUserIdAsync(userId);

            if (employee is null)
            {
                return new WeeklyProgressDTO(
                    0m,
                    40m,
                    0m
                );
            }

            // Use UTC
            var today = DateTime.UtcNow.Date;

            // Monday = start of week
            var daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;

            var weekStart =
                today.AddDays(-daysSinceMonday);

            // Exclusive end of week
            var weekEnd =
                weekStart.AddDays(7);

            var hoursWorked =
                await _attendanceRepository
                    .GetHoursWorkedForEmployeePeriodAsync(
                        employee.Id,
                        weekStart,
                        weekEnd);

            const decimal targetHours = 40m;

            var percentage =
                targetHours > 0
                    ? (hoursWorked / targetHours) * 100m
                    : 0m;

            var displayPercentage =
                Math.Min(percentage, 100m);

            return new WeeklyProgressDTO(
                Math.Round(hoursWorked, 2),
                targetHours,
                Math.Round(displayPercentage, 0)
            );
        }
    }
}
