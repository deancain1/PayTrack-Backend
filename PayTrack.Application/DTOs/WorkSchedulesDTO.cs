using PayTrack.Domain.Enums;
using System;

namespace PayTrack.Application.DTOs
{
    public record WorkSchedulesDTO(
        Guid Id,
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
    );
}