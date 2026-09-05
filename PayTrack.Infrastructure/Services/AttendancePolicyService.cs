using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;

namespace PayTrack.Infrastructure.Services
{
    public class AttendancePolicyService : IAttendancePolicyService
    {
        public string DetermineStatus(WorkSchedules schedule, DateTime timeIn)
        {
            switch (schedule.ScheduleType)
            {
                case ScheduleType.Fixed:

                    if (!schedule.StartTime.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Fixed schedule must have a start time.");
                    }

                    var latestFixed = schedule.StartTime.Value.Add(
                        TimeSpan.FromMinutes(
                            schedule.GracePeriodMinutes));

                    return timeIn.TimeOfDay > latestFixed
                        ? "Late"
                        : "Present";

                case ScheduleType.Flexible:

                    return "Present";

                case ScheduleType.Hybrid:

                    if (!schedule.StartTime.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Hybrid schedule must have a start time.");
                    }

                    var latestHybrid = schedule.StartTime.Value.Add(
                        TimeSpan.FromMinutes(
                            schedule.GracePeriodMinutes));

                    return timeIn.TimeOfDay > latestHybrid
                        ? "Late"
                        : "Present";

                default:

                    return "Present";
            }
        }

        public DateTime CalculateExpectedTimeOut(
            WorkSchedules schedule,
            DateTime timeIn)
        {
            return schedule.ScheduleType switch
            {
                ScheduleType.Fixed =>
                    CalculateFixedTimeOut(schedule, timeIn),

                ScheduleType.Flexible =>
                    timeIn.AddHours(schedule.RequiredHours),

                ScheduleType.Hybrid =>
                    timeIn.AddHours(schedule.RequiredHours),

                _ =>
                    CalculateFixedTimeOut(schedule, timeIn)
            };
        }

        private DateTime CalculateFixedTimeOut(
            WorkSchedules schedule,
            DateTime timeIn)
        {
            if (!schedule.EndTime.HasValue)
            {
                throw new InvalidOperationException(
                    "Fixed schedule must have an end time.");
            }

            return timeIn.Date + schedule.EndTime.Value;
        }
    }
}