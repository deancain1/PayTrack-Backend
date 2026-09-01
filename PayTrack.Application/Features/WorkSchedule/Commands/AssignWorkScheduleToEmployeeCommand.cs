using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public record AssignWorkScheduleToEmployeeCommand
    (
        Guid EmployeeId,
        Guid WorkScheduleId,
        DateOnly ScheduleEffectiveDate
    ) : IRequest;
}
