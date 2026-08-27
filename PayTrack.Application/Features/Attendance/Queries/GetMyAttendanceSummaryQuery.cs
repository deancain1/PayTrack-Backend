using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public record GetMyAttendanceSummaryQuery(
      int Month,
      int Year
  ) : IRequest<MyAttendanceSummaryDTO>;
}
