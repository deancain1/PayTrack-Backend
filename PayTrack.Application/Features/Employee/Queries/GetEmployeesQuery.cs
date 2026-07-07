using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Queries
{
    public record GetEmployeesQuery(int PageNumber = 1, int? PageSize = null, string? Search = null, Guid? DepartmentId = null) : IRequest<PagedResult<EmployeeDTO>>;
}
