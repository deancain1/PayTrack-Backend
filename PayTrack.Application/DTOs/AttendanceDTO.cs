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
        string fullName,
        string? department,
        string employeeNumber,
        DateTime? timeIn,
        DateTime? timeOut,
        string status

    );
}
