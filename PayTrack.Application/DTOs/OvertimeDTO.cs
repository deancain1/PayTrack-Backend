using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record OvertimeDTO(
         Guid Id,
         Guid EmployeeId,
         string EmployeeName,
         string EmployeeNumber,
         string Department,
         DateTime Date,
         TimeSpan StartTime,
         TimeSpan EndTime,
         decimal RequestedHours,
         decimal ActualHours,
         decimal OvertimeRate,
         decimal OvertimePay,
         string? Reason,
         string? EmployeeRemarks,
         string? AdminRemarks,
         OvertimeSource Source,
         OvertimeEmployeeResponse EmployeeResponse,
         OvertimeStatus Status,
         string CreatedBy,
         DateTime CreatedAt,
         DateTime? RespondedAt,
         DateTime? CompletedAt,
         Guid? PayrollRecordId
     );
}
