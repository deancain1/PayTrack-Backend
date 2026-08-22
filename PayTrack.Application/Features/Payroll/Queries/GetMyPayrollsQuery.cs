using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Payroll.Queries
{
    public record GetMyPayrollsQuery: IRequest<List<EmployeePayslipDTO>>;
}
