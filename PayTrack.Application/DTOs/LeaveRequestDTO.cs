using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record LeaveRequestDTO(
        Guid Id,
        Guid EmployeeId,
        string EmployeeName,
        string EmployeeNumber,
        string? DepartmentName,
        string LeaveType,
        DateTime StartDate,
        DateTime EndDate,
        int TotalDays,
        string Reason,
        string? AttachmentUrl,
        string Status,
        string? AdminRemarks,
        string? ReviewedBy,
        DateTime? ReviewedAt,
        DateTime CreatedAt
    );
}
