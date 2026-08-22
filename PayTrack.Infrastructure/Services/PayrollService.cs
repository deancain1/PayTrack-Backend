using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

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

     
        // REGULAR PAY
       
        public decimal Calculate(
            EmployeeSalary salary,
            Attendances attendance,
            WorkSchedules schedule)
        {
            decimal earned = 0;

            if (!attendance.TimeOut.HasValue)
                return 0;

            var regularHours =
                Math.Min(
                    attendance.HoursWorked,
                    schedule.RequiredHours);

            var scheduledEnd =
                attendance.Date +
                schedule.EndTime;

            bool completedShift =
                attendance.TimeOut.Value >= scheduledEnd;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:

                    earned =
                        salary.Rate *
                        regularHours;

                    break;

                case PayrollPolicy.ProRated:

                    earned =
                        (salary.Rate /
                         schedule.RequiredHours)
                        * regularHours;

                    break;

                case PayrollPolicy.FullDay:

                    if (completedShift)
                        earned = salary.Rate;

                    break;

                case PayrollPolicy.HalfDay:

                    if (completedShift)
                    {
                        earned = salary.Rate;
                    }
                    else if (
                        regularHours >=
                        schedule.RequiredHours / 2m)
                    {
                        earned =
                            salary.Rate / 2m;
                    }

                    break;

                default:

                    throw new InvalidOperationException(
                        "Unsupported payroll policy.");
            }

            return Math.Round(
                earned,
                2);
        }

    
        // CURRENT EARNINGS

        public async Task<decimal> CalculateCurrentEarningsAsync(
            Guid employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            var employee =
                await _employeesRepository
                    .GetByIdAsync(employeeId);

            if (employee == null)
                return 0;

            var salary =
                await _salaryRepository
                    .GetByEmployeeIdAsync(employeeId);

            if (salary == null)
                return 0;

            var schedule =
                employee.WorkSchedule;

            if (schedule == null)
                return 0;

            var attendances =
                await _attendanceRepository
                    .GetByEmployeeAndDateRangeAsync(
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

            return Math.Round(
                total,
                2);
        }

     
        // NORMAL HOURLY RATE

        public decimal CalculateHourlyRate(
            EmployeeSalary salary,
            WorkSchedules schedule)
        {
            if (schedule.RequiredHours <= 0)
            {
                throw new InvalidOperationException(
                    "Required working hours must be greater than zero.");
            }

            decimal hourlyRate;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:

                    hourlyRate =
                        salary.Rate;

                    break;

                case PayrollPolicy.ProRated:

                    hourlyRate =
                        salary.Rate /
                        schedule.RequiredHours;

                    break;

                case PayrollPolicy.FullDay:

                    hourlyRate =
                        salary.Rate /
                        schedule.RequiredHours;

                    break;

                case PayrollPolicy.HalfDay:

                    hourlyRate =
                        salary.Rate /
                        schedule.RequiredHours;

                    break;

                default:

                    throw new InvalidOperationException(
                        "Unsupported payroll policy.");
            }

            return Math.Round(
                hourlyRate,
                2);
        }

     
        // OVERTIME RATE
  

        public decimal CalculateOvertimeRate(
            EmployeeSalary salary,
            WorkSchedules schedule,
            decimal overtimeMultiplier)
        {
            if (overtimeMultiplier <= 0)
            {
                throw new InvalidOperationException(
                    "Overtime multiplier must be greater than zero.");
            }

            var hourlyRate =
                CalculateHourlyRate(
                    salary,
                    schedule);

            return Math.Round(
                hourlyRate *
                overtimeMultiplier,
                2);
        }

       
        // OVERTIME PAY
        public decimal CalculateOvertimePay(
            EmployeeSalary salary,
            WorkSchedules schedule,
            decimal actualHours,
            decimal overtimeMultiplier)
        {
            if (actualHours <= 0)
                return 0;

            var overtimeRate =
                CalculateOvertimeRate(
                    salary,
                    schedule,
                    overtimeMultiplier);

            return Math.Round(
                overtimeRate *
                actualHours,
                2);
        }
    }
}