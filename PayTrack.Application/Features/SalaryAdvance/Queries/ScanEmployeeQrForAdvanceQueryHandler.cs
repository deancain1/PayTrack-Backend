using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Queries
{
    public class ScanEmployeeQrForAdvanceQueryHandler : IRequestHandler<ScanEmployeeQrForAdvanceQuery, ScanEmployeeForAdvanceDTO>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;

        public ScanEmployeeQrForAdvanceQueryHandler(IEmployeesRepository employeesRepository, ISalaryAdvanceRepository salaryAdvanceRepository, IPayrollRecordRepository payrollRecordRepository)
        {
            _employeesRepository = employeesRepository;
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _payrollRecordRepository = payrollRecordRepository;

        }

        public async  Task<ScanEmployeeForAdvanceDTO> Handle(ScanEmployeeQrForAdvanceQuery request, CancellationToken cancellationToken)
        {
            var employee =
           await _employeesRepository.GetByIdAsync(request.EmployeeId);

            if (employee is null)
                throw new Exception("Employee not found.");

            var startDate = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            var endDate = DateTime.Today;

           
            decimal currentEarnings =
             await _payrollRecordRepository.GetTotalEarningsAsync(
                 employee.Id,
                 startDate,
                 endDate);

            decimal alreadyAdvanced =
                await _salaryAdvanceRepository.GetTotalAdvancesAsync(
                    employee.ApplicationUserId,
                    startDate,
                    endDate);

            decimal availableAdvance =
                currentEarnings - alreadyAdvanced;

            if (availableAdvance < 0)
                availableAdvance = 0;

            return new ScanEmployeeForAdvanceDTO( 
                employee.Id,
                employee.ApplicationUserId,
                employee.EmployeeNumber,
                employee.ApplicationUser.FullName,
                employee.Department?.DepartmentName ?? string.Empty,
                currentEarnings,
                alreadyAdvanced,
                availableAdvance
                
            );
        }
    }
}
