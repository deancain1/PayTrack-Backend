using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public record  UpdateWorkScheduleCommand
    (
        Guid Id,
        string ScheduleName,
        int ScheduleType,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int RequiredHours,
        int GracePeriodMinutes,
        TimeSpan AbsentCutoffTime
    ) : IRequest<WorkSchedulesDTO>;
}
