using MediatR;
using PayTrack.Application.Features.Notification.Events;
using PayTrack.Application.Features.Overtime.Commands;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Attendance.Commands
{
    public class RecordAttendanceCommandHandler
        : IRequestHandler<RecordAttendanceCommand, Guid>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IPhotoStorageService _photoStorageService;
        private readonly ISalaryRepository _salaryRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollService _payrollService;
        private readonly IAttendancePolicyService _attendancePolicyService;
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IMediator _mediator;

        public RecordAttendanceCommandHandler(
            IEmployeesRepository employeesRepository,
            IAttendanceRepository attendanceRepository,
            IPhotoStorageService photoStorageService,
            ISalaryRepository salaryRepository,
            IPayrollRecordRepository payrollRecordRepository,
            IPayrollService payrollService,
            IAttendancePolicyService attendancePolicyService,
            IAttendancePolicyRepository attendancePolicyRepository,
            IOvertimeRepository overtimeRepository,
            IMediator mediator)
        {
            _employeesRepository = employeesRepository;
            _attendanceRepository = attendanceRepository;
            _photoStorageService = photoStorageService;
            _salaryRepository = salaryRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _payrollService = payrollService;
            _attendancePolicyService = attendancePolicyService;
            _attendancePolicyRepository = attendancePolicyRepository;
            _overtimeRepository = overtimeRepository;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(
            RecordAttendanceCommand request,
            CancellationToken cancellationToken)
        {
            var employee =
                await _employeesRepository
                    .GetByIdAndNumberAsync(
                        request.Id,
                        request.EmployeeNumber);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee not found.");
            }

            var schedule =
                employee.WorkSchedule;

            if (schedule == null)
            {
                throw new InvalidOperationException(
                    "Employee work schedule is not assigned.");
            }

            var photoPath =
                await _photoStorageService
                    .SaveAttendancePhotoAsync(
                        request.PhotoBase64);

            var attendance =
                await _attendanceRepository
                    .GetTodayAttendanceAsync(
                        employee.Id);

            // Time In
            if (attendance == null)
            {
                var now =
                    DateTime.UtcNow;

                if (!schedule.StartTime.HasValue)
                {
                    throw new InvalidOperationException(
                        "Work schedule start time is not configured.");
                }

                var scheduledStart =
                    now.Date +
                    schedule.StartTime.Value;

                var latestAllowed =
                    scheduledStart.AddMinutes(
                        schedule.GracePeriodMinutes);

                bool isLate = false;

                decimal lateMinutes = 0;

                if (now > latestAllowed)
                {
                    isLate = true;

                    lateMinutes =
                        Math.Ceiling(
                            (decimal)(
                                now -
                                latestAllowed
                            ).TotalMinutes);
                }

                var status =
                    _attendancePolicyService
                        .DetermineStatus(
                            schedule,
                            now);

                attendance =
                    new Attendances
                    {
                        Id =
                            Guid.NewGuid(),

                        EmployeeId =
                            employee.Id,

                        Date =
                            now.Date,

                        TimeIn =
                            now,

                        TimeInPhoto =
                            photoPath,

                        TimeOut =
                            null,

                        TimeOutPhoto =
                            null,

                        HoursWorked =
                            0,

                        IsLate =
                            isLate,

                        LateMinutes =
                            lateMinutes,

                        Status =
                            status
                    };

                await _attendanceRepository
                    .AddAsync(attendance);

                // Convert UTC time to Philippine time only for display
                var philippinesTimeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Manila");

                var timeIn =
                    TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(
                            attendance.TimeIn.Value,
                            DateTimeKind.Utc),
                        philippinesTimeZone);

                await _mediator.Publish(
                    new CompanyAdminNotificationCreatedEvent(
                        Guid.NewGuid(),
                        employee.CompanyAdminId,
                        CompanyAdminNotificationType.TimeIn,
                        $"{employee.ApplicationUser.FullName} has timed in at {timeIn:hh:mm tt}.",
                        DateTime.UtcNow
                    ),
                    cancellationToken);

                return attendance.Id;
            }

            // Time Out
            if (attendance.TimeOut == null)
            {
                if (!attendance.TimeIn.HasValue)
                {
                    throw new InvalidOperationException(
                        "Employee has no recorded Time In.");
                }

                var timeOut =
                    DateTime.UtcNow;

                if (timeOut <
                    attendance.TimeIn.Value)
                {
                    throw new InvalidOperationException(
                        "Time Out cannot be earlier than Time In.");
                }

                attendance.TimeOut =
                    timeOut;

                attendance.TimeOutPhoto =
                    photoPath;

                var duration =
                    timeOut -
                    attendance.TimeIn.Value;

                var elapsedHours =
                    Math.Round(
                        (decimal)duration.TotalHours,
                        2);

                var regularHours =
                    Math.Min(
                        elapsedHours,
                        schedule.RequiredHours);

                attendance.HoursWorked =
                    Math.Round(
                        regularHours,
                        2);

                await _attendanceRepository
                    .UpdateAsync(
                        attendance);

                var salary =
                    employee.Salary;

                if (salary == null)
                {
                    throw new InvalidOperationException(
                        "Employee salary not configured.");
                }

                var earned =
                    _payrollService.Calculate(
                        salary,
                        attendance,
                        schedule);

                var policy =
                    employee.UseCompanyLatePolicy
                        ? await _attendancePolicyRepository
                            .GetDefaultPolicyAsync(
                                employee.CompanyAdminId)
                        : employee.AttendancePolicyId.HasValue
                            ? await _attendancePolicyRepository
                                .GetByIdAsync(
                                    employee.AttendancePolicyId.Value)
                            : null;

                if (policy == null)
                {
                    throw new InvalidOperationException(
                        "Attendance policy not found.");
                }

                decimal deduction = 0;

                if (
                    policy.EnableLateDeduction &&
                    attendance.IsLate)
                {
                    switch (policy.LateDeductionType)
                    {
                        case LateDeductionType.None:

                            deduction = 0;

                            break;

                        case LateDeductionType.FixedAmount:

                            deduction =
                                policy.DeductionValue;

                            break;

                        case LateDeductionType.Percentage:

                            deduction =
                                earned *
                                (
                                    policy.DeductionValue /
                                    100m
                                );

                            break;

                        case LateDeductionType.PerMinute:

                            deduction =
                                attendance.LateMinutes *
                                policy.DeductionValue;

                            break;
                    }
                }

                var finalSalary =
                    earned -
                    deduction;

                if (finalSalary < 0)
                {
                    finalSalary = 0;
                }

                finalSalary =
                    Math.Round(
                        finalSalary,
                        2);

                var payroll =
                    new PayrollRecord
                    {
                        Id =
                            Guid.NewGuid(),

                        EmployeeId =
                            employee.Id,

                        ApplicationUserId =
                            employee.ApplicationUserId,

                        AttendanceId =
                            attendance.Id,

                        AmountEarned =
                            finalSalary,

                        Status =
                            PayrollRecordStatus.Unpaid,

                        CreatedAt =
                            DateTime.UtcNow
                    };

                await _payrollRecordRepository
                    .AddAsync(payroll);

                // Complete overtime if employee reached the overtime period
                var overtime =
                    await _overtimeRepository
                        .GetActiveOvertimeForEmployeeAsync(
                            employee.Id,
                            attendance.Date);

                if (overtime != null)
                {
                    // Overtime.StartTime is TimeSpan, not TimeSpan?
                    var overtimeStart =
                        overtime.Date.Date +
                        overtime.StartTime;

                    if (timeOut >= overtimeStart)
                    {
                        await _mediator.Send(
                            new CompleteOvertimeCommand(
                                overtime.Id,
                                timeOut),
                            cancellationToken);
                    }
                }

                // Convert UTC time to Philippine time only for display
                var philippinesTimeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Manila");

                var philippinesTimeOut =
                    TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(
                            attendance.TimeOut.Value,
                            DateTimeKind.Utc),
                        philippinesTimeZone);

                await _mediator.Publish(
                    new CompanyAdminNotificationCreatedEvent(
                        Guid.NewGuid(),
                        employee.CompanyAdminId,
                        CompanyAdminNotificationType.TimeOut,
                        $"{employee.ApplicationUser.FullName} has timed out at {philippinesTimeOut:hh:mm tt}.",
                        DateTime.UtcNow
                    ),
                    cancellationToken);

                return attendance.Id;
            }

            throw new InvalidOperationException(
                "Employee already timed in and timed out today.");
        }
    }
}