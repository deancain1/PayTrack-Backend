using MediatR;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollRecords.Queries
{
    internal class GetEmployeeTotalEarningsQueryHandler : IRequestHandler<GetEmployeeTotalEarningsQuery, decimal>
    {
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetEmployeeTotalEarningsQueryHandler(IPayrollRecordRepository payrollRecordRepository, IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _payrollRecordRepository = payrollRecordRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task<decimal> Handle(GetEmployeeTotalEarningsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository.GetCurrentEmployeeAsync(_currentUserService.UserId);

            if (employee == null)
                throw new Exception("Employee not found.");

            return await _payrollRecordRepository
                .GetTotalEarningsAsync(employee.Id);
        }
    }
}
