using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Commands
{
    public record RecordAttendanceCommand(
        Guid Id,
        string EmployeeNumber,
        string PhotoBase64
    ) : IRequest<Guid>;
}
