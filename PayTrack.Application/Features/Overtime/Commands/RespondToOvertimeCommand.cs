using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Overtime.Commands
{
    public record RespondToOvertimeCommand(
       Guid Id,
       bool Accept,
       string? Remarks
   ) : IRequest;
}
