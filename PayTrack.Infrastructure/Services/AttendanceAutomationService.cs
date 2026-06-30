using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class AttendanceAutomationService : IAttendanceAutomationService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceAutomationService(
            IEmployeesRepository employeesRepository,
            IAttendanceRepository attendanceRepository)
        {
            _employeesRepository = employeesRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task MarkAbsentEmployeesAsync()
        {
            var employees =
                await _employeesRepository.GetEmployeesForAbsentAutomationAsync();

            var now = DateTime.Now;

            foreach (var employee in employees)
            {
                if (employee.WorkSchedule == null)
                    continue;

                // Fixed Schedule only
                if (employee.WorkSchedule.ScheduleType != ScheduleType.Fixed)
                    continue;

                var schedule = employee.WorkSchedule;

                // Don't mark absent before the configured cutoff
                if (now.TimeOfDay < schedule.AbsentCutoffTime)
                    continue;

                // Already has attendance today?
                var attendance =
                    await _attendanceRepository
                        .GetTodayAttendanceAsync(employee.Id);

                if (attendance != null)
                    continue;

                attendance = new Attendances
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Date = now.Date,
                    Status = "Absent",
                    TimeIn = null,
                    TimeOut = null,
                    HoursWorked = 0
                };

                await _attendanceRepository.AddAsync(attendance);
            }
        }
    }
}
