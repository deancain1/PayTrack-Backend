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
        DateTime date,
        string? ProfilePictureBase64,
        string FullName,
        string? Department,
        string EmployeeNumber,
        DateTime? TimeIn,
        DateTime? TimeOut,
        string status,
        string? TimeInPhoto,
        string? TimeOutPhoto

    );
}
