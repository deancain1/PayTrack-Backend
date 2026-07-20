using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{
    public record ApproveSalaryAdvanceCommand(
         Guid Id
     ) : IRequest<bool>;
}
