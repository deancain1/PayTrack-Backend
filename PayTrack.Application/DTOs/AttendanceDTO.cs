using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record AttendanceDTO
    (
        Guid Id,
        string? PhotoBase64,
        string FullName,
        string Department,
        string EmployeeNumber,
        DateTime? TimeIn,
        DateTime? TimeOut,
        string Status

    );
}
