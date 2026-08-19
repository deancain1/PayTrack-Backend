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
            _publisher = publisher;
        }

        public async Task<GeneratePayrollResult> Handle(GeneratePayrollCommand request, CancellationToken cancellationToken)
        {


            if (request.PeriodStart.Date > request.PeriodEnd.Date)
            {
                throw new Exception(
                    "Payroll start date cannot be later than payroll end date.");
            }

            var periodStart = request.PeriodStart.Date;
            var periodEnd = request.PeriodEnd.Date;

            // STEP 2: CHECK FOR SKIPPED PREVIOUS ATTENDANCE


            var hasSkippedAttendance =
                await _payrollRecordRepository
                    .HasUnpaidAttendanceBeforePeriodAsync(
                        periodStart);

            if (hasSkippedAttendance)
            {
                throw new Exception(
                    "Cannot generate payroll. An employee has previous attendance with unpaid earnings. Please process the previous attendance first.");
            }


            // STEP 3: FIND EXISTING OVERLAPPING ACTIVE PAYROLL

            var existingBatch =
                await _payrollBatchRepository
                    .GetOverlappingActiveBatchAsync(
                        periodStart,
                        periodEnd);

            // STEP 4: DEACTIVATE EXISTING OVERLAPPING BATCH

            if (existingBatch != null)
            {
                existingBatch.IsActive = false;

                _payrollBatchRepository.Update(existingBatch);

                await _payrollBatchRepository.SaveChangesAsync();
            }

            // STEP 5: CREATE NEW PAYROLL BATCH

            var batch = new PayrollBatch
            {
                Id = Guid.NewGuid(),
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _payrollBatchRepository.AddAsync(batch);

            // STEP 6: GET EMPLOYEES

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


            // STEP 7: GET COMPANY PAYROLL SETTINGS


            var companyAdminId =
                _currentUserService.UserId;

            var payrollSetting =
                await _companyPayrollSettingRepository
                    .GetByCompanyAdminIdAsync(
                        companyAdminId);

            if (payrollSetting == null)
            {
                throw new Exception(
                    "Payroll settings have not been configured.");
            }

            var payrollFrequency =
                payrollSetting.Frequency;

            // STEP 8: GENERATE PAYROLL FOR EACH EMPLOYEE

            var employeeResults =
                new List<EmployeePayrollDTO>();

            foreach (var employee in employees)
            {
                cancellationToken.ThrowIfCancellationRequested();



                var employeeSalary =
                    await _salaryRepository
                        .GetByEmployeeIdAsync(
                            employee.Id);

                if (employeeSalary == null)
                {
                    throw new Exception(
                        $"No salary assigned to {employee.ApplicationUser.FullName}");
                }

                var monthlyRate =
                    employeeSalary.MonthlyRate;


                var totalAmount =
                    await _payrollRecordRepository
                        .GetTotalEarningsAsync(
                            employee.Id,
                            periodStart,
                            periodEnd);


                if (totalAmount <= 0)
                {
                    continue;
                }




                var salaryAdvanceDeduction =
                    await _salaryAdvanceRepository
                        .GetTotalUndeductedAdvancesAsync(
                            employee.Id);

                var philHealthDeduction =
                    await _philHealthService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);

                var pagIbigDeduction =
                    await _pagIbigService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);


                var sssDeduction =
                    await _sssService
                        .ComputeEmployeeContributionAsync(
                            monthlyRate,
                            payrollFrequency);


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


                var payroll = new Payrolls
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    PayrollBatchId = batch.Id,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    TotalAmount = totalAmount,
                    SalaryAdvanceDeduction = salaryAdvanceDeduction,
                    PhilHealthDeduction = philHealthDeduction,
                    PagIbigDeduction = pagIbigDeduction,
                    SSSDeduction = sssDeduction,
                    NetAmount = netAmount,
                    Status = PayrollStatus.Pending,
                    CreatedAt = DateTime.UtcNow
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
                    record.PayrollId = payroll.Id;

                    _payrollRecordRepository.Update(record);
                }


                employeeResults.Add(
                    new EmployeePayrollDTO(
                        employee.Id,
                        employee.ApplicationUser.FullName,
                        totalAmount,
                        salaryAdvanceDeduction,
                        philHealthDeduction,
                        pagIbigDeduction,
                        sssDeduction,
                        netAmount));
            }



            if (!employeeResults.Any())
            {
                batch.IsActive = false;

                _payrollBatchRepository.Update(batch);

                await _payrollBatchRepository
                    .SaveChangesAsync();

                throw new Exception(
                    "No employees have payable earnings for this payroll period.");
            }


            await _payrollBatchRepository
                .SaveChangesAsync();


            foreach (var employee in employees)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var payrollWasGenerated =
                    employeeResults.Any(
                        x => x.EmployeeId == employee.Id);

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