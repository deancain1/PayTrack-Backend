using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
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

        public GetEmployeesLookupQueryHandler(
            IEmployeesRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<EmployeeLookupDTO>> Handle(GetEmployeesLookupQuery request, CancellationToken cancellationToken)
        {

           return await _employeeRepository
          .GetEmployeesQuery()
          .Select(e => new EmployeeLookupDTO(
            e.Id,
            e.ApplicationUserId,
            e.ApplicationUser.FullName,
            e.EmployeeNumber,
            e.Department != null? e.Department.DepartmentName
              : "Unassigned"
           ))
          .ToListAsync(cancellationToken);
        }
    }
}
