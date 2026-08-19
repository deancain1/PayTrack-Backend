using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record WorkSchedulesDTO(
     Guid Id,
     string ScheduleName,
     int ScheduleType,
     TimeSpan StartTime,
     TimeSpan EndTime,
     int RequiredHours,
     int GracePeriodMinutes,
     TimeSpan AbsentCutoffTime,
     WorkingDay WorkingDays
 );
}
