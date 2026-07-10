using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public record AssignAttendancePolicyCommand
    (
      Guid EmployeeId,
      Guid AttendancePolicyId,
      bool UseCompanyPolicy
    ) : IRequest;
}
