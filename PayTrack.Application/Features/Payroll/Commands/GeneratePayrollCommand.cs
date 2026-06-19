using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Payroll.Commands
{
    public record GeneratePayrollCommand(List<Guid>? EmployeeIds, DateTime PeriodStart, DateTime PeriodEnd) : IRequest<GeneratePayrollResult>;
}
