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
        // REGULAR PAY
        public decimal Calculate(
            EmployeeSalary salary,
            Attendances attendance,
            WorkSchedules schedule,
            AttendancePolicy? attendancePolicy = null)
        {
            if (!attendance.TimeIn.HasValue)
                return 0m;

            if (!attendance.TimeOut.HasValue)
                return 0m;

            if (schedule.RequiredHours <= 0)
                throw new InvalidOperationException(
                    "Required working hours must be greater than zero.");

            if (!schedule.StartTime.HasValue)
                throw new InvalidOperationException(
                    "Work schedule start time is not configured.");

            if (!schedule.EndTime.HasValue)
                throw new InvalidOperationException(
                    "Work schedule end time is not configured.");

            // Philippine timezone
            var philippinesTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "Asia/Manila");

            // Attendance timestamps are stored in UTC.
            var timeInUtc =
                DateTime.SpecifyKind(
                    attendance.TimeIn.Value,
                    DateTimeKind.Utc);

            var timeOutUtc =
                DateTime.SpecifyKind(
                    attendance.TimeOut.Value,
                    DateTimeKind.Utc);

            // Convert attendance times to Philippine time
            // for schedule comparisons.
            var timeInPhilippines =
                TimeZoneInfo.ConvertTimeFromUtc(
                    timeInUtc,
                    philippinesTimeZone);

            var timeOutPhilippines =
                TimeZoneInfo.ConvertTimeFromUtc(
                    timeOutUtc,
                    philippinesTimeZone);

            // Scheduled times are Philippine local times.
            var scheduledStart =
                timeInPhilippines.Date +
                schedule.StartTime.Value;

            var scheduledEnd =
                timeInPhilippines.Date +
                schedule.EndTime.Value;

            // Determine whether the employee arrived late.
            bool isLate =
                timeInPhilippines >
                scheduledStart.AddMinutes(
                    schedule.GracePeriodMinutes);

            // Determine whether the employee completed
            // the scheduled shift.
            //
            // Early Time-In does not move the scheduled
            // end time.
            bool completedShift =
                timeOutPhilippines >=
                scheduledEnd;

            // Actual hours worked are calculated from
            // the stored UTC timestamps.
            decimal actualHours =
                Math.Max(
                    0m,
                    attendance.HoursWorked);

            // Never pay more than the required regular hours.
            decimal regularHours =
                Math.Min(
                    actualHours,
                    schedule.RequiredHours);

            decimal earned = 0m;

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

                    // Reaching the scheduled end time
                    // completes the scheduled shift.
                    //
                    // Example:
                    // 8:30 AM -> 4:00 PM = FullDay salary
                    //
                    // Late deduction is handled separately.
                    if (completedShift)
                    {
                        earned =
                            salary.Rate;
                    }
                    else if (
                        regularHours >=
                        schedule.RequiredHours)
                    {
                        earned =
                            salary.Rate;
                    }

                    break;

                case PayrollPolicy.HalfDay:

                    if (completedShift)
                    {
                        earned =
                            salary.Rate;
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

            // Apply late deduction only when:
            //
            // 1. Attendance policy exists
            // 2. Late deduction is enabled
            // 3. Deduction type is not None
            // 4. Employee actually arrived late
            bool applyLateDeduction =
                attendancePolicy != null &&
                attendancePolicy.EnableLateDeduction &&
                attendancePolicy.LateDeductionType !=
                    LateDeductionType.None &&
                isLate;

            if (applyLateDeduction)
            {
                // Calculate actual minutes late
                // from the scheduled start time.
                var lateMinutes =
                    Math.Max(
                        0m,
                        (decimal)(
                            timeInPhilippines -
                            scheduledStart
                        ).TotalMinutes);

                decimal deduction = 0m;

                switch (attendancePolicy!.LateDeductionType)
                {
                    case LateDeductionType.FixedAmount:

                        deduction =
                            attendancePolicy.DeductionValue;

                        break;

                    case LateDeductionType.Percentage:

                        deduction =
                            earned *
                            (
                                attendancePolicy.DeductionValue /
                                100m
                            );

                        break;

                    case LateDeductionType.PerMinute:

                        deduction =
                            lateMinutes *
                            attendancePolicy.DeductionValue;

                        break;

                    case LateDeductionType.None:

                    default:

                        deduction = 0m;

                        break;
                }

                // Salary should never become negative.
                earned =
                    Math.Max(
                        0m,
                        earned - deduction);
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
                return 0m;

            var salary =
                await _salaryRepository
                    .GetByEmployeeIdAsync(employeeId);

            if (salary == null)
                return 0m;

            var schedule =
                employee.WorkSchedule;

            if (schedule == null)
                return 0m;

            var attendances =
                await _attendanceRepository
                    .GetByEmployeeAndDateRangeAsync(
                        employeeId,
                        startDate,
                        endDate);

            decimal total = 0m;

            // Get the employee attendance policy.
            var attendancePolicy =
                employee.AttendancePolicy;

            foreach (var attendance in attendances)
            {
                total += Calculate(
                    salary,
                    attendance,
                    schedule,
                    attendancePolicy);
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
                return 0m;

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

        // PAID LEAVE
        public decimal CalculatePaidLeave(
            EmployeeSalary salary,
            WorkSchedules schedule,
            int paidLeaveDays)
        {
            if (paidLeaveDays <= 0)
                return 0m;

            var dailyRate =
                CalculateDailyRate(
                    salary,
                    schedule);

            return Math.Round(
                dailyRate *
                paidLeaveDays,
                2);
        }

        // DAILY RATE
        public decimal CalculateDailyRate(
            EmployeeSalary salary,
            WorkSchedules schedule)
        {
            if (schedule.RequiredHours <= 0)
                throw new InvalidOperationException(
                    "Required working hours must be greater than zero.");

            return salary.Policy switch
            {
                PayrollPolicy.Hourly =>
                    Math.Round(
                        salary.Rate *
                        schedule.RequiredHours,
                        2),

                PayrollPolicy.FullDay =>
                    Math.Round(
                        salary.Rate,
                        2),

                PayrollPolicy.ProRated =>
                    Math.Round(
                        salary.Rate,
                        2),

                PayrollPolicy.HalfDay =>
                    Math.Round(
                        salary.Rate / 2m,
                        2),

                _ =>
                    0m
            };
        }
    }
}