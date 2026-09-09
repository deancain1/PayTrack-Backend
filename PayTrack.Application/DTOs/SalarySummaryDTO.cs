using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalarySummaryDTO(
    int FullDayCount,
    decimal FullDayTotal,

    int HourlyCount,
    decimal HourlyTotal,

    int ProRatedCount,
    decimal ProRatedTotal
);
}
