using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Commands
{
    public record SetEmployeePasswordCommand(
      string UserId,
      string Token,
      string NewPassword
  ) : IRequest<string>;
}
