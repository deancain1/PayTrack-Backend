using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public record CancelLeaveRequestCommand(
        Guid LeaveRequestId
    ) : IRequest<bool>;
}

