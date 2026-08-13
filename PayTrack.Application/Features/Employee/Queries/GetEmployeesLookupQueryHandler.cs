using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.AttendancePolicies.Queries;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Queries
{
    public class GetEmployeesLookupQueryHandler: IRequestHandler<GetEmployeesLookupQuery, List<EmployeeLookupDTO>>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetEmployeesLookupQueryHandler(IEmployeesRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<EmployeeLookupDTO>> Handle(GetEmployeesLookupQuery request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;
            var employees = await _employeeRepository.GetAllEmployeesByCompanyAdminAsync(companyAdminId);
       
             return employees
                .Select(e => new EmployeeLookupDTO(
                    e.Id,
                    e.ApplicationUserId,
                    e.ApplicationUser.FullName,
                    e.EmployeeNumber,
                    e.Department != null
                        ? e.Department.DepartmentName
                        : "Unassigned"
                ))
                .ToList();
        }
    }
}
