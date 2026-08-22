using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public record CreateCompanyAdminOvertimeCommand(
        Guid EmployeeId,
        DateTime Date,
        TimeSpan StartTime,
        TimeSpan EndTime,
        string? Reason
    ) : IRequest<Guid>;
}
