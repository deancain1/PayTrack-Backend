using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{
    public class CreateSalaryAdvanceCommandHandler
      : IRequestHandler<CreateSalaryAdvanceCommand, Guid>
    {
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;

        private readonly IEmployeesRepository _employeesRepository;

        private readonly IPayrollRecordRepository _payrollRecordRepository;

        public CreateSalaryAdvanceCommandHandler(
            ISalaryAdvanceRepository salaryAdvanceRepository,
            IEmployeesRepository employeesRepository,
            IPayrollRecordRepository payrollRecordRepository)
        {
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _employeesRepository = employeesRepository;
            _payrollRecordRepository = payrollRecordRepository;
        }

        public async Task<Guid> Handle(
            CreateSalaryAdvanceCommand request,
            CancellationToken cancellationToken)
        {
            var employee =
                await _employeesRepository.GetByIdAsync(request.EmployeeId);

            if (employee == null)
                throw new Exception("Employee not found.");

            var startDate = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            var endDate = DateTime.Now;

            decimal currentEarnings =
          await _payrollRecordRepository.GetTotalEarningsAsync(
              employee.Id,
              startDate,
              endDate);

            decimal alreadyAdvanced =
                await _salaryAdvanceRepository.GetTotalAdvancesAsync(
                    employee.Id,
                    startDate,
                    endDate);

            decimal available =
                currentEarnings - alreadyAdvanced;

            if (request.Amount > available)
                throw new Exception(
                    $"Maximum advance available is {available:N2}");

            var advance = new SalaryAdvances
            {
                Id = Guid.NewGuid(),

                EmployeeId = employee.Id,

                Amount = request.Amount,

                RequestDate = DateTime.Now,

                Remarks = request.Remarks,

                
            };

            await _salaryAdvanceRepository.AddAsync(advance);

            return advance.Id;
        }
    }
}
