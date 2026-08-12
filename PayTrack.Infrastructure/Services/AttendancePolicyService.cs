using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class AttendancePolicyService
     : IAttendancePolicyService
    {
        public string DetermineStatus(WorkSchedules schedule, DateTime timeIn)
        {
            switch (schedule.ScheduleType)
            {
                case ScheduleType.Fixed:

                    var latestFixed =
                        schedule.StartTime.Add(
                            TimeSpan.FromMinutes(
                                schedule.GracePeriodMinutes));

                    return timeIn.TimeOfDay > latestFixed
                        ? "Late"
                        : "Present";

                case ScheduleType.Flexible:

                    return "Present";

                case ScheduleType.Hybrid:

                    var latestHybrid =
                        schedule.StartTime.Add(
                            TimeSpan.FromMinutes(
                                schedule.GracePeriodMinutes));

                    return timeIn.TimeOfDay > latestHybrid
                        ? "Late"
                        : "Present";

                default:
                    return "Present";
            }
        }

        public DateTime CalculateExpectedTimeOut(WorkSchedules schedule, DateTime timeIn)
        {
            return schedule.ScheduleType switch
            {
                ScheduleType.Fixed =>
                    timeIn.Date + schedule.EndTime,

                ScheduleType.Flexible =>
                    timeIn.AddHours(
                        schedule.RequiredHours),

                ScheduleType.Hybrid =>
                    timeIn.AddHours(
                        schedule.RequiredHours),

                _ =>
                    timeIn.Date + schedule.EndTime
            };
        }
    }
}
