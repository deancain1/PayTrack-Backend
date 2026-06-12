using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Queries
{
    public record GetEmployeeSalaryQuery(int PageNumber = 1, int? PageSize = null, string? Search = null) : IRequest<PagedResult<SalaryDTO>>;
  
}
