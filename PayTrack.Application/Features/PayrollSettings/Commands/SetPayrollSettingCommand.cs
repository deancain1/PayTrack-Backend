using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollSettings.Commands
{
    public record SetPayrollSettingCommand(
         PayrollFrequency Frequency
     ) : IRequest<bool>;
}
