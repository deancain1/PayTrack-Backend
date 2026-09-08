using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Notification.Events;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Payroll.Commands
{
    public class GeneratePayrollCommandHandler
        : IRequestHandler<GeneratePayrollCommand, GeneratePayrollResult>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollBatchRepository _payrollBatchRepository;
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly ISalaryRepository _salaryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyPayrollSettingRepository _companyPayrollSettingRepository;
        private readonly IPhilHealthService _philHealthService;
        private readonly IPagIbigService _pagIbigService;
        private readonly ISSSService _sssService;
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IPayrollService _payrollService;
        private readonly IPublisher _publisher;

        public GeneratePayrollCommandHandler(
            IEmployeesRepository employeesRepository,
            IPayrollRecordRepository payrollRecordRepository,
            IPayrollRepository payrollRepository,
            IPayrollBatchRepository payrollBatchRepository,
            ISalaryAdvanceRepository salaryAdvanceRepository,
            ISalaryRepository salaryRepository,
            ICurrentUserService currentUserService,
            ICompanyPayrollSettingRepository companyPayrollSettingRepository,
            IPhilHealthService philHealthService,
            IPagIbigService pagIbigService,
            ISSSService sssService,
            IOvertimeRepository overtimeRepository,
            ILeaveRequestRepository leaveRequestRepository,
            IPayrollService payrollService,
            IPublisher publisher)
        {
            _employeesRepository = employeesRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _payrollRepository = payrollRepository;
            _payrollBatchRepository = payrollBatchRepository;
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _salaryRepository = salaryRepository;
            _currentUserService = currentUserService;
            _companyPayrollSettingRepository = companyPayrollSettingRepository;
            _philHealthService = philHealthService;
            _pagIbigService = pagIbigService;
            _sssService = sssService;
            _overtimeRepository = overtimeRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _payrollService = payrollService;
            _publisher = publisher;
        }

        public async Task<GeneratePayrollResult> Handle(GeneratePayrollCommand request, CancellationToken cancellationToken)
        {
            if (request.PeriodStart.Date > request.PeriodEnd.Date)
            {
                throw new InvalidOperationException(
                    "Payroll start date cannot be later than payroll end date.");
            }

            // Convert payroll period dates to UTC
            var periodStart = DateTime.SpecifyKind(
                request.PeriodStart.Date,
                DateTimeKind.Utc);

            var periodEnd = DateTime.SpecifyKind(
                request.PeriodEnd.Date,
                DateTimeKind.Utc);

            var hasSkippedAttendance =
                await _payrollRecordRepository
                    .HasUnpaidAttendanceBeforePeriodAsync(
                        periodStart);

            if (hasSkippedAttendance)
            {
                throw new InvalidOperationException(
                    "Cannot generate payroll. An employee has previous attendance with unpaid earnings. Please process the previous attendance first.");
            }

            var existingBatch =
                await _payrollBatchRepository
                    .GetOverlappingActiveBatchAsync(
                        periodStart,
                        periodEnd);

            if (existingBatch != null)
            {
                existingBatch.IsActive = false;

                _payrollBatchRepository.Update(existingBatch);

                await _payrollBatchRepository
                    .SaveChangesAsync();
            }

            var batch = new PayrollBatch
            {
                Id = Guid.NewGuid(),
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _payrollBatchRepository.AddAsync(batch);

            List<Employees> employees;

            if (request.EmployeeIds != null &&
                request.EmployeeIds.Any())
            {
                employees =
                    await _employeesRepository
                        .GetByIdsAsync(request.EmployeeIds);
            }
            else
            {
                employees =
                    await _employeesRepository
                        .GetAllAsync();
            }

            var companyAdminId =
                _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(companyAdminId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var payrollSetting =
                await _companyPayrollSettingRepository
                    .GetByCompanyAdminIdAsync(companyAdminId);

            if (payrollSetting == null)
            {
                throw new InvalidOperationException(
                    "Payroll settings have not been configured.");
            }

            var payrollFrequency =
                payrollSetting.Frequency;

            var employeeResults =
                new List<EmployeePayrollDTO>();

            foreach (var employee in employees)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var employeeSalary =
                    await _salaryRepository
                        .GetByEmployeeIdAsync(employee.Id);

                if (employeeSalary == null)
                {
                    throw new InvalidOperationException(
                        $"No salary assigned to {employee.ApplicationUser.FullName}");
                }

                var schedule =
                    employee.WorkSchedule;

                if (schedule == null)
                {
                    throw new InvalidOperationException(
                        $"No work schedule assigned to {employee.ApplicationUser.FullName}");
                }

                var monthlyRate =
                    employeeSalary.MonthlyRate;

                var regularAmount =
                    await _payrollRecordRepository
                        .GetTotalEarningsAsync(
                            employee.Id,
                            periodStart,
                            periodEnd);

                regularAmount =
                    Math.Round(regularAmount, 2);



                var paidLeaves =
                    await _leaveRequestRepository
                        .GetApprovedPaidLeaveAsync(
                            employee.Id,
                            periodStart,
                            periodEnd);

                var paidLeaveDays = 0;

                foreach (var leave in paidLeaves)
                {
                    var leaveStart =
                        leave.StartDate.Date < periodStart.Date
                            ? periodStart.Date
                            : leave.StartDate.Date;

                    var leaveEnd =
                        leave.EndDate.Date > periodEnd.Date
                            ? periodEnd.Date
                            : leave.EndDate.Date;

                    if (leaveEnd >= leaveStart)
                    {
                        paidLeaveDays +=
                            (leaveEnd - leaveStart).Days + 1;
                    }
                }

                var paidLeaveAmount =
                    _payrollService.CalculatePaidLeave(
                        employeeSalary,
                        schedule,
                        paidLeaveDays);

                paidLeaveAmount =
                    Math.Round(paidLeaveAmount, 2);

                var completedOvertimes =
                    await _overtimeRepository
                        .GetCompletedOvertimesForPayrollAsync(
                            employee.Id,
                            periodStart,
                            periodEnd);

                var overtimeAmount =
                    completedOvertimes.Sum(
                        x => x.OvertimePay);

                overtimeAmount =
                    Math.Round(overtimeAmount, 2);


                var totalAmount =
                    Math.Round(
                        regularAmount +
                        paidLeaveAmount +
                        overtimeAmount,
                        2);

                if (totalAmount <= 0)
                {
                    continue;
                }



                var salaryAdvanceDeduction =
                    await _salaryAdvanceRepository
                        .GetTotalUndeductedAdvancesAsync(
                            employee.Id);

                salaryAdvanceDeduction =
                    Math.Round(
                        salaryAdvanceDeduction,
                        2);



                var philHealthDeduction =
                    await _philHealthService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);

                philHealthDeduction =
                    Math.Round(
                        philHealthDeduction,
                        2);



                var pagIbigDeduction =
                    await _pagIbigService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);

                pagIbigDeduction =
                    Math.Round(
                        pagIbigDeduction,
                        2);



                var sssDeduction =
                    await _sssService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);

                sssDeduction =
                    Math.Round(
                        sssDeduction,
                        2);



                var netAmount =
                    totalAmount
                    - salaryAdvanceDeduction
                    - philHealthDeduction
                    - pagIbigDeduction
                    - sssDeduction;

                if (netAmount < 0)
                {
                    netAmount = 0;
                }

                netAmount =
                    Math.Round(netAmount, 2);


                var payroll = new Payrolls
                {
                    Id = Guid.NewGuid(),

                    EmployeeId =
                        employee.Id,

                    PayrollBatchId =
                        batch.Id,

                    PeriodStart =
                        periodStart,

                    PeriodEnd =
                        periodEnd,

                    RegularAmount =
                        regularAmount,

                    OvertimeAmount =
                        overtimeAmount,

                    PaidLeaveAmount =
                        paidLeaveAmount,

                    TotalAmount =
                        totalAmount,

                    SalaryAdvanceDeduction =
                        salaryAdvanceDeduction,

                    PhilHealthDeduction =
                        philHealthDeduction,

                    PagIbigDeduction =
                        pagIbigDeduction,

                    SSSDeduction =
                        sssDeduction,

                    NetAmount =
                        netAmount,

                    Status =
                        PayrollStatus.Pending,

                    CreatedAt =
                        DateTime.UtcNow
                };

                await _payrollRepository
                    .AddAsync(payroll);



                var dailyRecords =
                    await _payrollRecordRepository
                        .GetUnassignedPayrollRecordsAsync(
                            employee.Id,
                            periodStart,
                            periodEnd);

                foreach (var record in dailyRecords)
                {
                    record.PayrollId =
                        payroll.Id;

                    _payrollRecordRepository
                        .Update(record);
                }



                foreach (var overtime in completedOvertimes)
                {
                    overtime.PayrollId =
                        payroll.Id;

                    _overtimeRepository
                        .Update(overtime);
                }


                employeeResults.Add(
                    new EmployeePayrollDTO(
                        EmployeeId:
                            employee.Id,

                        EmployeeName:
                            employee.ApplicationUser.FullName,

                        RegularAmount:
                            regularAmount,

                        OvertimeAmount:
                            overtimeAmount,

                        PaidLeaveAmount:
                            paidLeaveAmount,

                        TotalAmount:
                            totalAmount,

                        SalaryAdvance:
                            salaryAdvanceDeduction,

                        PhilHealthDeduction:
                            philHealthDeduction,

                        PagIbigDeduction:
                            pagIbigDeduction,

                        SssDeduction:
                            sssDeduction,

                        NetAmount:
                            netAmount
                    ));
            }


            if (!employeeResults.Any())
            {
                batch.IsActive = false;

                _payrollBatchRepository
                    .Update(batch);

                await _payrollBatchRepository
                    .SaveChangesAsync();

                throw new InvalidOperationException(
                    "No employees have payable earnings for this payroll period.");
            }

            await _payrollBatchRepository
                .SaveChangesAsync();



            foreach (var employee in employees)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var payrollWasGenerated =
                    employeeResults.Any(
                        x =>
                            x.EmployeeId ==
                            employee.Id);

                if (!payrollWasGenerated)
                {
                    continue;
                }

                await _publisher.Publish(
                    new EmployeeNotificationCreatedEvent(
                        Guid.NewGuid(),
                        employee.ApplicationUserId,
                        EmployeeNotificationType.PayrollGenerated,
                        "Payroll Generated",
                        $"Your payroll for {periodStart:MMM dd, yyyy} - {periodEnd:MMM dd, yyyy} has been generated."
                    ),
                    cancellationToken);
            }



            return new GeneratePayrollResult(
                GrandTotal:
                    employeeResults.Sum(
                        x => x.NetAmount),

                EmployeeCount:
                    employeeResults.Count,

                Employees:
                    employeeResults);
        }
    }
}