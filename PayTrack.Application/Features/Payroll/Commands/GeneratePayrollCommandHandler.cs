using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Payroll.Commands
{
    public class GeneratePayrollCommandHandler: IRequestHandler<GeneratePayrollCommand, GeneratePayrollResult>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollBatchRepository _payrollBatchRepository;

        public GeneratePayrollCommandHandler(
            IEmployeesRepository employeesRepository,
            IPayrollRecordRepository payrollRecordRepository,
            IPayrollRepository payrollRepository, IPayrollBatchRepository payrollBatchRepository)
        {
            _employeesRepository = employeesRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _payrollRepository = payrollRepository;
            _payrollBatchRepository = payrollBatchRepository;
        }

        public async Task<GeneratePayrollResult> Handle(
        GeneratePayrollCommand request,
        CancellationToken cancellationToken)
        {
            // STEP 1: Deactivate old batch for same period
            var existingBatch =
                await _payrollBatchRepository.GetByPeriodAsync(
                    request.PeriodStart,
                    request.PeriodEnd);

            if (existingBatch != null)
            {
                existingBatch.IsActive = false;
                await _payrollBatchRepository.UpdateAsync(existingBatch);
            }

            // STEP 2: Create new batch
            var batch = new PayrollBatch
            {
                Id = Guid.NewGuid(),
                PeriodStart = request.PeriodStart.Date,
                PeriodEnd = request.PeriodEnd.Date,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _payrollBatchRepository.AddAsync(batch);

            // STEP 3: Get employees
            var employees = new List<Employees>();

            if (request.EmployeeIds != null && request.EmployeeIds.Any())
            {
                employees = await _employeesRepository
                    .GetByIdsAsync(request.EmployeeIds);
            }
            else
            {
                employees = await _employeesRepository.GetAllAsync();
            }

            // STEP 4: Build result
            var employeeResults = new List<EmployeePayrollDTO>();

            foreach (var employee in employees)
            {
                var totalAmount =
                    await _payrollRecordRepository.GetTotalEarningsAsync(
                        employee.Id,
                        request.PeriodStart,
                        request.PeriodEnd);

                if (totalAmount <= 0)
                    continue;

                var payroll = new Payrolls
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    PayrollBatchId = batch.Id,
                    PeriodStart = request.PeriodStart.Date,
                    PeriodEnd = request.PeriodEnd.Date,
                    TotalAmount = totalAmount,
                    Status = PayrollStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _payrollRepository.AddAsync(payroll);

                employeeResults.Add(new EmployeePayrollDTO(
                    employee.Id,
                    employee.ApplicationUser.FullName,
                    totalAmount
                ));
            }

            // STEP 5: FINAL RESULT (IMPORTANT FIX)
            return new GeneratePayrollResult(
                GrandTotal: employeeResults.Sum(x => x.TotalAmount),
                EmployeeCount: employeeResults.Count,
                Employees: employeeResults
            );
        }
    }
}

