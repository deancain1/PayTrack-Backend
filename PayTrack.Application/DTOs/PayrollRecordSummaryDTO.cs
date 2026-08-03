using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record PayrollRecordSummaryDTO(
     decimal TotalPayrollToday,
     decimal AverageDailyPayout,
     int ActiveTracked
 );
}
