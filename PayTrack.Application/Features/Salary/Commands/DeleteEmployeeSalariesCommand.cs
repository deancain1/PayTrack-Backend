using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public record DeleteEmployeeSalariesCommand
    (
        List<Guid> EmployeeSalaryIds
    ) : IRequest<Unit>;
   
}
