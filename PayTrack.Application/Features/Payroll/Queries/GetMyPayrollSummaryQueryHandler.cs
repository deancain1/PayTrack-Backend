using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Payroll.Queries
{
    public class GetMyPayrollSummaryQueryHandler : IRequestHandler<GetMyPayrollSummaryQuery, EmployeePayrollSummaryDTO>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;

        public GetMyPayrollSummaryQueryHandler(
            ICurrentUserService currentUserService,
            IEmployeesRepository employeesRepository,
            IPayrollRecordRepository payrollRecordRepository,
            ISalaryAdvanceRepository salaryAdvanceRepository)
        {
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _salaryAdvanceRepository = salaryAdvanceRepository;
        }
        public async Task<EmployeePayrollSummaryDTO> Handle(GetMyPayrollSummaryQuery request, CancellationToken cancellationToken)
        {
            var employee = 
                       _currentUserService.UserId;

            if (employee == null)
                throw new Exception("Employee not found.");

            var startDate = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            var endDate = DateTime.Now;

            var grossEarnings =
                await _payrollRecordRepository
                    .GetTotalUnpaidEarningsAsync(
                        _currentUserService.UserId);


            var totalAdvanced =
                await _salaryAdvanceRepository.GetTotalAdvancesAsync(
                    _currentUserService.UserId,
                    startDate,
                    endDate);

            var availableEarnings = grossEarnings - totalAdvanced;

            if (availableEarnings < 0)
                availableEarnings = 0;

            return new EmployeePayrollSummaryDTO(
            grossEarnings,
            totalAdvanced,
            availableEarnings
            );
        }
    }
}
