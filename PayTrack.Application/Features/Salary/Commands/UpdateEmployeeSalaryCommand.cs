using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public record UpdateEmployeeSalaryCommand
    (
        Guid Id,
        decimal rate,
        PayrollPolicy policy
    ): IRequest<bool>;
}
