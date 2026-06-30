using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public record AssignScheduleToEmployeeCommand
    (
        Guid EmployeeId,
        Guid WorkScheduleId,
        DateTime ScheduleEffectiveDate
    ) : IRequest;
}
