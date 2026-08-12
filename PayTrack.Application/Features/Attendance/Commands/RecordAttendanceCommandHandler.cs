using MediatR;
using PayTrack.Application.Features.Notification.Events;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Attendance.Commands
{
    public class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, Guid>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IPhotoStorageService _photoStorageService;
        private readonly ISalaryRepository _salaryRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollService _payrollService;
        private readonly IAttendancePolicyService _attendancePolicyService;
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
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
            _mediator = mediator;
        }

        public async Task<Guid> Handle(RecordAttendanceCommand request,CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository
                .GetByIdAndNumberAsync(request.Id, request.EmployeeNumber);

            if (employee == null)
                throw new Exception("Employee not found.");

            var photoPath = await _photoStorageService
                .SaveAttendancePhotoAsync(request.PhotoBase64);

            var attendance = await _attendanceRepository
                .GetTodayAttendanceAsync(employee.Id);

           
            // FIRST SCAN = TIME IN
            if (attendance == null)
            {
                var now = DateTime.Now;

                var schedule = employee.WorkSchedule!;
                var scheduledStart = now.Date + schedule.StartTime;
                var latestAllowed = scheduledStart.AddMinutes(schedule.GracePeriodMinutes);

                DateTime effectiveTimeIn = now;
                bool isLate = false;
                decimal lateMinutes = 0;

           
                // GRACE PERIOD LOGIC
          
                if (now <= latestAllowed)
                {
                    // NOT LATE → treat as scheduled start
                    effectiveTimeIn = scheduledStart;
                }
                else
                {
                    // LATE → compute late minutes
                    isLate = true;
                    lateMinutes = Math.Ceiling((decimal)(now - latestAllowed).TotalMinutes);
                }

                var status = _attendancePolicyService
                    .DetermineStatus(schedule, now);

                attendance = new Attendances
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Status = status,
                    Date = now.Date,
                    TimeIn = now,
                    TimeInPhoto = photoPath,

                    
                    IsLate = isLate,
                    LateMinutes = lateMinutes
                };

                await _attendanceRepository.AddAsync(attendance);

                await _mediator.Publish(new NotificationCreatedEvent(
                 Guid.NewGuid(),
                 employee.CompanyAdminId,
                 CompanyAdminNotificationType.TimeIn,
                 $"{employee.ApplicationUser.FullName} has timed in at {attendance.TimeIn:hh:mm tt}.",
                 DateTime.UtcNow
                ));

                return attendance.Id;
            }

           
            // SECOND SCAN = TIME OUT
            
            if (attendance.TimeOut == null)
            {
                if (!attendance.TimeIn.HasValue)
                    throw new Exception("Employee has no recorded Time In.");

                var timeOut = DateTime.Now;

                attendance.TimeOut = timeOut;
                attendance.TimeOutPhoto = photoPath;

                var schedule = employee.WorkSchedule!;
                var scheduledStart = attendance.Date + schedule.StartTime;

              
                // Use scheduled start OR time-in logic 
                DateTime effectiveTimeIn = attendance.IsLate
                    ? attendance.TimeIn.Value
                    : scheduledStart;

                var duration = timeOut - effectiveTimeIn;

                attendance.HoursWorked = Math.Round(
                    (decimal)duration.TotalHours,
                    2);

                await _attendanceRepository.UpdateAsync(attendance);

               
                // PAYROLL
               

                var salary = employee.Salary;

                if (salary == null)
                    throw new Exception("Employee salary not configured.");

                if (employee.WorkSchedule == null)
                    throw new Exception("Employee work schedule not assigned.");

                var requiredHours = employee.WorkSchedule.RequiredHours;

                var earned = _payrollService.Calculate(salary, attendance, employee.WorkSchedule!);

               
                // LOAD ATTENDANCE POLICY
              

                var policy = employee.UseCompanyLatePolicy
                    ? await _attendancePolicyRepository.GetDefaultPolicyAsync(employee.CompanyAdminId)
                    : await _attendancePolicyRepository.GetByIdAsync(employee.AttendancePolicyId!.Value);

                if (policy == null)
                    throw new Exception("Attendance policy not found.");

             
                // LATE DEDUCTION
            

                decimal deduction = 0;

                if (policy.EnableLateDeduction && attendance.IsLate)
                {
                    switch (policy.LateDeductionType)
                    {
                        case LateDeductionType.None:
                            deduction = 0;
                            break;

                        case LateDeductionType.FixedAmount:
                            deduction = policy.DeductionValue;
                            break;

                        case LateDeductionType.Percentage:
                            deduction = earned * (policy.DeductionValue / 100);
                            break;

                        case LateDeductionType.PerMinute:
                            deduction = attendance.LateMinutes * policy.DeductionValue;
                            break;
                    }
                }

                var finalSalary = earned - deduction;

                if (finalSalary < 0)
                    finalSalary = 0;

                var payroll = new PayrollRecord
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    ApplicationUserId = employee.ApplicationUserId,
                    AttendanceId = attendance.Id,
                    AmountEarned = finalSalary,
                    Status = PayrollRecordStatus.Unpaid,
                    CreatedAt = DateTime.Now
                };

                await _payrollRecordRepository.AddAsync(payroll);

                await _mediator.Publish(new NotificationCreatedEvent(
                Guid.NewGuid(),
                employee.CompanyAdminId,
                CompanyAdminNotificationType.TimeOut,
                $"{employee.ApplicationUser.FullName} has timed out at {attendance.TimeOut:hh:mm tt}.",
                DateTime.UtcNow
                ));
                return attendance.Id;
            }

            throw new Exception("Employee already timed in and timed out today.");
        }
    }
}
