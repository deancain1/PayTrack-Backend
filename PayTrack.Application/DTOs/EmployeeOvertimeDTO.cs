using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeeOvertimeDTO(
        Guid Id,
        DateTime Date,
        TimeSpan StartTime,
        TimeSpan EndTime,
        decimal RequestedHours,
        string? Reason,
        OvertimeSource Source,
        OvertimeEmployeeResponse EmployeeResponse,
        OvertimeStatus Status,
        string? EmployeeRemarks,
        string? AdminRemarks,
        DateTime CreatedAt,
        DateTime? RespondedAt
    );
}
