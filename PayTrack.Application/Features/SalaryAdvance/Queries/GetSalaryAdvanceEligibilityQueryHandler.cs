using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Queries
{
    public class GetSalaryAdvanceEligibilityQueryHandler : IRequestHandler<GetSalaryAdvanceEligibilityQuery, SalaryAdvanceEligibilityDTO>
    {
        private readonly ICurrentUserService _currentUserService;  
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ISalaryAdvanceEligibilityService _salaryAdvanceEligibilityService;

        public GetSalaryAdvanceEligibilityQueryHandler(
            ICurrentUserService currentUserService,
            IEmployeesRepository employeesRepository,
            ISalaryAdvanceEligibilityService salaryAdvanceEligibilityService)
        {
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
            _salaryAdvanceEligibilityService = salaryAdvanceEligibilityService;
        }
        public async  Task<SalaryAdvanceEligibilityDTO> Handle(GetSalaryAdvanceEligibilityQuery request, CancellationToken cancellationToken)
        {
            var employee =
                 await _employeesRepository
                     .GetCurrentEmployeeAsync(
                         _currentUserService.UserId);

            if (employee == null)
            {
                throw new Exception(
                    "Employee not found.");
            }

            return await _salaryAdvanceEligibilityService
                .GetEligibilityAsync(employee.Id);
        }
    }
}
