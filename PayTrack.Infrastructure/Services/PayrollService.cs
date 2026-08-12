using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ISalaryRepository _salaryRepository;
        public PayrollService(
            IAttendanceRepository attendanceRepository,
            IEmployeesRepository employeesRepository,
            ISalaryRepository salaryRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeesRepository = employeesRepository;
           _salaryRepository = salaryRepository;
        }
        public decimal Calculate(EmployeeSalary salary, Attendances attendance, WorkSchedules schedule)
        {
            decimal earned = 0;

            var scheduledEnd = attendance.Date + schedule.EndTime;

            bool completedShift =
                attendance.TimeOut.HasValue &&
                attendance.TimeOut.Value >= scheduledEnd;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:

                    earned = salary.Rate * attendance.HoursWorked;

                    break;

                case PayrollPolicy.ProRated:

                    earned =
                        (salary.Rate / schedule.RequiredHours)
                        * attendance.HoursWorked;

                    break;

                case PayrollPolicy.FullDay:

                    if (completedShift)
                        earned = salary.Rate;
                    else
                        earned = 0;

                    break;

                case PayrollPolicy.HalfDay:

                    if (completedShift)
                        earned = salary.Rate;
                    else if (attendance.HoursWorked >= schedule.RequiredHours / 2m)
                        earned = salary.Rate / 2m;
                    else
                        earned = 0;

                    break;

                case PayrollPolicy.Monthly:

                    earned = salary.Rate;

                    break;
            }

            return Math.Round(earned, 2);
        }

        public async Task<decimal> CalculateCurrentEarningsAsync(
    Guid employeeId,
    DateTime startDate,
    DateTime endDate)
        {
            var employee =
                await _employeesRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return 0;

            var salary =
                await _salaryRepository.GetByEmployeeIdAsync(employeeId);

            if (salary == null)
                return 0;

            var schedule = employee.WorkSchedule;

            if (schedule == null)
                return 0;

            var attendances =
                await _attendanceRepository.GetByEmployeeAndDateRangeAsync(
                    employeeId,
                    startDate,
                    endDate);

            decimal total = 0;

            foreach (var attendance in attendances)
            {
                total += Calculate(
                    salary,
                    attendance,
                    schedule);
            }

            return Math.Round(total, 2);
        }
    }
}
