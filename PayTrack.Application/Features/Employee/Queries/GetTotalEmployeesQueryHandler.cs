using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Queries
{
    public class GetTotalEmployeesQueryHandler : IRequestHandler<GetTotalEmployeesQuery, EmployeeStatsDTO>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetTotalEmployeesQueryHandler(IEmployeesRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<EmployeeStatsDTO> Handle(GetTotalEmployeesQuery request, CancellationToken cancellationToken)
        {
           var totalEmployees = await _employeeRepository.GetTotalEmployeesByCompanyAdminAsync(_currentUserService.UserId);

            return new EmployeeStatsDTO(
              totalEmployees
          );
        }
    }
}
