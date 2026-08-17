using MediatR;
using Microsoft.AspNetCore.Http;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public record CreateLeaveRequestCommand(
      Guid EmployeeId,
      string LeaveType,
      DateTime StartDate,
      DateTime EndDate,
      string Reason,
      IFormFile? Attachment
  ) : IRequest<LeaveRequestDTO>;
}
