using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{
    public sealed record CreateSalaryAdvanceRequestCommand(
     decimal Amount,
     string? Remarks
 ) : IRequest<Guid>;
}
