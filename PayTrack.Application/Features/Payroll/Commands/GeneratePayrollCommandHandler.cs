using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Payroll.Commands
{
    public class GeneratePayrollCommandHandler :
        IRequestHandler<GeneratePayrollCommand, GeneratePayrollResult>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollBatchRepository _payrollBatchRepository;
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;


        public GeneratePayrollCommandHandler(
            IEmployeesRepository employeesRepository,
            IPayrollRecordRepository payrollRecordRepository,
            IPayrollRepository payrollRepository,
            IPayrollBatchRepository payrollBatchRepository,
            ISalaryAdvanceRepository salaryAdvanceRepository)
        {
            _employeesRepository = employeesRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _payrollRepository = payrollRepository;
            _payrollBatchRepository = payrollBatchRepository;
            _salaryAdvanceRepository = salaryAdvanceRepository;
        }


        public async Task<GeneratePayrollResult> Handle(
            GeneratePayrollCommand request,
            CancellationToken cancellationToken)
        {

            if (request.PeriodStart > request.PeriodEnd)
            {
                throw new Exception(
                    "Payroll start date cannot be later than payroll end date.");
            }

            // STEP 1: Check Existing Same Period
            // Allow regeneration

            var existingBatch =
                await _payrollBatchRepository.GetByPeriodAsync(
                    request.PeriodStart,
                    request.PeriodEnd);



            if (existingBatch != null)
            {
                existingBatch.IsActive = false;

                await _payrollBatchRepository
                    .UpdateAsync(existingBatch);
            }

            // STEP 2: Prevent Overlapping Payroll

            var hasOverlap =
                await _payrollBatchRepository
                .HasOverlappingBatchAsync(
                    request.PeriodStart,
                    request.PeriodEnd,
                    existingBatch?.Id);


            if (hasOverlap)
            {
                throw new Exception(
                    "Payroll period overlaps with an existing payroll.");
            }


            // STEP 3: Prevent Skipping Unpaid Attendance

            var hasUnpaidAttendance =
                await _payrollRecordRepository
                .HasUnpaidAttendanceBeforePeriodAsync(
                    request.PeriodStart);


            if (hasUnpaidAttendance)
            {
                throw new Exception(
                    "Cannot generate payroll. There are unpaid attendance records before this payroll period.");
            }



            // STEP 4: Create Payroll Batch

            var batch = new PayrollBatch
            {
                Id = Guid.NewGuid(),

                PeriodStart = request.PeriodStart.Date,

                PeriodEnd = request.PeriodEnd.Date,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };


            await _payrollBatchRepository
                .AddAsync(batch);

            // STEP 5: Get Employees

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


            // STEP 6: Generate Payroll
 

            var employeeResults =
                new List<EmployeePayrollDTO>();


            foreach (var employee in employees)
            {

                var totalAmount =
                    await _payrollRecordRepository
                    .GetTotalEarningsAsync(
                        employee.Id,
                        request.PeriodStart,
                        request.PeriodEnd);



                if (totalAmount <= 0)
                    continue;



                var salaryAdvanceDeduction =
                    await _salaryAdvanceRepository
                    .GetTotalUndeductedAdvancesAsync(
                        employee.Id);



                var netAmount =
                    totalAmount - salaryAdvanceDeduction;



                if (netAmount < 0)
                    netAmount = 0;



                var payroll = new Payrolls
                {
                    Id = Guid.NewGuid(),

                    EmployeeId = employee.Id,

                    PayrollBatchId = batch.Id,

                    PeriodStart = request.PeriodStart.Date,

                    PeriodEnd = request.PeriodEnd.Date,

                    TotalAmount = totalAmount,

                    SalaryAdvanceDeduction =
                        salaryAdvanceDeduction,

                    NetAmount = netAmount,

                    Status = PayrollStatus.Pending,

                    CreatedAt = DateTime.UtcNow
                };



                await _payrollRepository
                    .AddAsync(payroll);



                employeeResults.Add(
                    new EmployeePayrollDTO(
                        employee.Id,
                        employee.ApplicationUser.FullName,
                        totalAmount,
                        salaryAdvanceDeduction,
                        netAmount
                    ));
            }

            // STEP 7: Return Result

            return new GeneratePayrollResult(
                GrandTotal:
                    employeeResults.Sum(x => x.NetAmount),

                EmployeeCount:
                    employeeResults.Count,

                Employees:
                    employeeResults
            );
        }
    }
}