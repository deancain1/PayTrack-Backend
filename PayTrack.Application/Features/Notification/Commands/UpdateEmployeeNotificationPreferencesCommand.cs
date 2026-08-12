using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Commands
{
    public record UpdateEmployeeNotificationPreferencesCommand(
         bool SalaryAdvanceApproved,
         bool SalaryAdvanceRejected,
         bool PayrollGenerated,
         bool PayrollReleased
     ) : IRequest<bool>;
}
