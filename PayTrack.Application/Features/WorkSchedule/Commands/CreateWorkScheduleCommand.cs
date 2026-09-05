using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Domain.Enums;
using System;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public record CreateWorkScheduleCommand(
        string ScheduleName,
        int ScheduleType,
        TimeSpan? StartTime,
        TimeSpan? EndTime,
        TimeSpan? EarliestStartTime,
        TimeSpan? LatestStartTime,
        int RequiredHours,
        int GracePeriodMinutes,
        TimeSpan? AbsentCutoffTime,
        WorkingDay WorkingDays
    ) : IRequest<WorkSchedulesDTO>;
}