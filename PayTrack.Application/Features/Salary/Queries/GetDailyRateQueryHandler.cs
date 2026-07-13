using MediatR;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Queries
{
    public class GetDailyRateQueryHandler : IRequestHandler<GetDailyRateQuery, decimal>
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetDailyRateQueryHandler(ISalaryRepository salaryRepository, IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _salaryRepository = salaryRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }
        public async Task<decimal> Handle(GetDailyRateQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository
          .GetCurrentEmployeeAsync(_currentUserService.UserId);

            if (employee == null)
                throw new Exception("Employee not found.");

            return await _salaryRepository
                .GetDailyRateAsync(employee.Id);
        }
    }
}
