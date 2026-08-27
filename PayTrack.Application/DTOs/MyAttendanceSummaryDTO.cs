using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record MyAttendanceSummaryDTO(
        int TotalPresent,
        int TotalLate,
        int TotalAbsent,
        double AverageHours
    );
}
