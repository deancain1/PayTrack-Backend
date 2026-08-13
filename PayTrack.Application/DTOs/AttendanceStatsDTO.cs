
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record AttendanceStatsDTO(
       int PresentToday,
       int LateToday,
       int AbsentToday
   )
    {
        public int TotalToday =>
            PresentToday + LateToday + AbsentToday;

        public double PresentPercentage =>
            TotalToday == 0 ? 0 : (double)PresentToday / TotalToday * 100;

        public double LatePercentage =>
            TotalToday == 0 ? 0 : (double)LateToday / TotalToday * 100;

        public double AbsentPercentage =>
            TotalToday == 0 ? 0 : (double)AbsentToday / TotalToday * 100;
    }
}
