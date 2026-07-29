using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Commands
{
    public record UpdateDepartmentCommand
    (
        Guid Id,
        string departmentName,
        string description
    ) : IRequest<bool>;
}
