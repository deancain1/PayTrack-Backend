using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public record ApproveLeaveRequestCommand(
         Guid LeaveRequestId,
         bool IsPaid,
         string? Remarks
     ) : IRequest<bool>;
}
