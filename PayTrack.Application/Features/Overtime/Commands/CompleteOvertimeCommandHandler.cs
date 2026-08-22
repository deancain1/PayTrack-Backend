using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public class CompleteOvertimeCommandHandler
        : IRequestHandler<CompleteOvertimeCommand>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ISalaryRepository _salaryRepository;
        private readonly IPayrollService _payrollService;
        private readonly ICurrentUserService _currentUserService;

        public CompleteOvertimeCommandHandler(
            IOvertimeRepository overtimeRepository,
            ISalaryRepository salaryRepository,
            IPayrollService payrollService,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _salaryRepository = salaryRepository;
            _payrollService = payrollService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(
            CompleteOvertimeCommand request,
            CancellationToken cancellationToken)
        {
            // ============================================================
            // CURRENT USER
            // ============================================================

            var userId =
                _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            // ============================================================
            // GET OVERTIME
            // ============================================================

            var overtime =
                await _overtimeRepository
                    .GetByIdWithApplicationUserAsync(
                        request.Id);

            if (overtime == null)
            {
                throw new KeyNotFoundException(
                    "Overtime request not found.");
            }

            // ============================================================
            // ONLY ACCEPTED OT CAN BE COMPLETED
            // ============================================================

            if (overtime.Status !=
                OvertimeStatus.Accepted)
            {
                throw new InvalidOperationException(
                    "Only accepted overtime can be completed.");
            }

            // ============================================================
            // EMPLOYEE
            // ============================================================

            var employee =
                overtime.Employee;

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee not found.");
            }

            // ============================================================
            // AUTHORIZATION
            //
            // The employee who owns the OT or their company admin
            // can complete it.
            // ============================================================

            var isEmployee =
                employee.ApplicationUserId == userId;

            var isAdmin =
                employee.CompanyAdminId == userId;

            if (!isEmployee && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to complete this overtime.");
            }

            // ============================================================
            // VALIDATE TIME OUT
            // ============================================================

            if (request.TimeOut < overtime.Date.Date)
            {
                throw new InvalidOperationException(
                    "Invalid overtime Time Out.");
            }

            // ============================================================
            // OVERTIME START
            // ============================================================

            var overtimeStart =
                overtime.Date.Date +
                overtime.StartTime;

            // ============================================================
            // OVERTIME END
            // ============================================================

            var overtimeEnd =
                overtime.Date.Date +
                overtime.EndTime;

            // ============================================================
            // EMPLOYEE DID NOT ACTUALLY REACH OT
            // ============================================================

            if (request.TimeOut <= overtimeStart)
            {
                throw new InvalidOperationException(
                    "The employee did not complete any overtime.");
            }

            // ============================================================
            // IMPORTANT:
            //
            // If employee works beyond approved OT,
            // only the approved OT period is payable.
            //
            // Example:
            //
            // Approved: 5 PM - 7 PM
            // TimeOut:  8 PM
            //
            // Actual payable OT = 2 hours
            //
            // NOT 3 hours.
            // ============================================================

            var actualEnd =
                request.TimeOut < overtimeEnd
                    ? request.TimeOut
                    : overtimeEnd;

            // ============================================================
            // CALCULATE ACTUAL OT HOURS
            // ============================================================

            var actualHours =
                (decimal)(
                    actualEnd -
                    overtimeStart
                ).TotalHours;

            if (actualHours <= 0)
            {
                throw new InvalidOperationException(
                    "Actual overtime hours must be greater than zero.");
            }

            // ============================================================
            // NEVER EXCEED REQUESTED HOURS
            // ============================================================

            actualHours =
                Math.Min(
                    actualHours,
                    overtime.RequestedHours);

            actualHours =
                Math.Round(
                    actualHours,
                    2);

            // ============================================================
            // GET SALARY
            // ============================================================

            var salary =
                await _salaryRepository
                    .GetByEmployeeIdAsync(
                        overtime.EmployeeId);

            if (salary == null)
            {
                throw new InvalidOperationException(
                    "Employee salary was not found.");
            }

            // ============================================================
            // WORK SCHEDULE
            // ============================================================

            var schedule =
                employee.WorkSchedule;

            if (schedule == null)
            {
                throw new InvalidOperationException(
                    "Employee work schedule was not found.");
            }

            // ============================================================
            // OT MULTIPLIER
            //
            // For now:
            // 1.25 = 125%
            //
            // Later you can move this to company payroll settings.
            // ============================================================

            const decimal overtimeMultiplier =
                1.25m;

            // ============================================================
            // CALCULATE OT RATE
            // ============================================================

            var overtimeRate =
                _payrollService
                    .CalculateOvertimeRate(
                        salary,
                        schedule,
                        overtimeMultiplier);

            // ============================================================
            // CALCULATE OT PAY
            // ============================================================

            var overtimePay =
                _payrollService
                    .CalculateOvertimePay(
                        salary,
                        schedule,
                        actualHours,
                        overtimeMultiplier);

            // ============================================================
            // UPDATE OT
            // ============================================================

            overtime.ActualHours =
                actualHours;

            overtime.OvertimeRate =
                Math.Round(
                    overtimeRate,
                    2);

            overtime.OvertimePay =
                Math.Round(
                    overtimePay,
                    2);

            overtime.Status =
                OvertimeStatus.Completed;

            overtime.CompletedAt =
                DateTime.UtcNow;


            _overtimeRepository.Update(
                overtime);

            await _overtimeRepository.SaveChangesAsync();
        }
    }
}