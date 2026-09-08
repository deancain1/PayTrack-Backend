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

            var schedule = employee.WorkSchedule;

            if (schedule == null)
            {
                throw new InvalidOperationException(
                    "Employee work schedule is not assigned.");
            }

            if (string.IsNullOrWhiteSpace(request.PhotoBase64))
            {
                throw new InvalidOperationException(
                    "Attendance photo is required.");
            }

            var philippinesTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "Asia/Manila");

            var attendance =
                await _attendanceRepository
                    .GetTodayAttendanceAsync(
                        employee.Id);

            var nowUtc =
                DateTime.UtcNow;

            var nowPhilippines =
                TimeZoneInfo.ConvertTimeFromUtc(
                    nowUtc,
                    philippinesTimeZone);

            // Time In
            if (attendance == null)
            {
                if (!schedule.StartTime.HasValue)
                {
                    throw new InvalidOperationException(
                        "Work schedule start time is not configured.");
                }

                var scheduledStart =
                    schedule.StartTime.Value;

                var latestAllowed =
                    scheduledStart.Add(
                        TimeSpan.FromMinutes(
                            schedule.GracePeriodMinutes));

                var isLate =
                    nowPhilippines.TimeOfDay >
                    latestAllowed;

                decimal lateMinutes = 0m;

                if (isLate)
                {
                    lateMinutes =
                        Math.Ceiling(
                            (decimal)(
                                nowPhilippines.TimeOfDay -
                                scheduledStart
                            ).TotalMinutes);
                }

                var status =
                    _attendancePolicyService
                        .DetermineStatus(
                            schedule,
                            nowPhilippines);

                var photoPath =
                    await _photoStorageService
                        .SaveAttendancePhotoAsync(
                            request.PhotoBase64);

                attendance =
                    new Attendances
                    {
                        Id =
                            Guid.NewGuid(),

                        EmployeeId =
                            employee.Id,

                        // Date is stored as UTC.
                        // The Philippine date is already
                        // used when determining today's attendance.
                        Date =
                            DateTime.SpecifyKind(
                                nowPhilippines.Date,
                                DateTimeKind.Utc),

                        // Timestamp is always UTC.
                        TimeIn =
                            nowUtc,

                        TimeInPhoto =
                            photoPath,

                        TimeOut =
                            null,

                        TimeOutPhoto =
                            null,

                        HoursWorked =
                            0m,

                        IsLate =
                            isLate,

                        LateMinutes =
                            lateMinutes,

                        Status =
                            status
                    };

                await _attendanceRepository
                    .AddAsync(
                        attendance);

                await _mediator.Publish(
                    new CompanyAdminNotificationCreatedEvent(
                        Guid.NewGuid(),
                        employee.CompanyAdminId,
                        CompanyAdminNotificationType.TimeIn,
                        $"{employee.ApplicationUser.FullName} has timed in at {nowPhilippines:hh:mm tt}.",
                        nowUtc),
                    cancellationToken);

                return attendance.Id;
            }

            // Time Out
            if (!attendance.TimeOut.HasValue)
            {
                if (!attendance.TimeIn.HasValue)
                {
                    throw new InvalidOperationException(
                        "Employee has no recorded Time In.");
                }

                var timeOutUtc =
                    DateTime.UtcNow;

                if (timeOutUtc <
                    attendance.TimeIn.Value)
                {
                    throw new InvalidOperationException(
                        "Time Out cannot be earlier than Time In.");
                }

                var photoPath =
                    await _photoStorageService
                        .SaveAttendancePhotoAsync(
                            request.PhotoBase64);

                attendance.TimeOut =
                    timeOutUtc;

                attendance.TimeOutPhoto =
                    photoPath;

                var duration =
                    timeOutUtc -
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
                        Math.Max(
                            0m,
                            regularHours),
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

                // PayrollService handles the salary
                // calculation and late deduction.
                var finalSalary =
                    _payrollService.Calculate(
                        salary,
                        attendance,
                        schedule,
                        policy);

                finalSalary =
                    Math.Round(
                        Math.Max(
                            0m,
                            finalSalary),
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
                    .AddAsync(
                        payroll);

                // Complete overtime
                var overtime =
                    await _overtimeRepository
                        .GetActiveOvertimeForEmployeeAsync(
                            employee.Id,
                            attendance.Date);

                if (overtime != null)
                {
                    var timeOutPhilippines =
                        TimeZoneInfo.ConvertTimeFromUtc(
                            timeOutUtc,
                            philippinesTimeZone);

                    var overtimeStartPhilippines =
                        overtime.Date.Date +
                        overtime.StartTime;

                    if (timeOutPhilippines >=
                        overtimeStartPhilippines)
                    {
                        await _mediator.Send(
                            new CompleteOvertimeCommand(
                                overtime.Id,
                                timeOutUtc),
                            cancellationToken);
                    }
                }

                var philippinesTimeOut =
                    TimeZoneInfo.ConvertTimeFromUtc(
                        attendance.TimeOut.Value,
                        philippinesTimeZone);

                await _mediator.Publish(
                    new CompanyAdminNotificationCreatedEvent(
                        Guid.NewGuid(),
                        employee.CompanyAdminId,
                        CompanyAdminNotificationType.TimeOut,
                        $"{employee.ApplicationUser.FullName} has timed out at {philippinesTimeOut:hh:mm tt}.",
                        DateTime.UtcNow),
                    cancellationToken);

                return attendance.Id;
            }

            throw new InvalidOperationException(
                "Employee already timed in and timed out today.");
        }
    }
}