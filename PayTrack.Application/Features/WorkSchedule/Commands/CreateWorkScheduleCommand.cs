using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{

    public record CreateWorkScheduleCommand(
        string ScheduleName,
        int ScheduleType,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int RequiredHours,
        int GracePeriodMinutes
    ) : IRequest<Guid>;
}
