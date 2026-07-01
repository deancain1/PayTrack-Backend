using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public record UpdateAttendancePolicyCommand(
        Guid Id,
        string CompanyAdminId,
        string Name,
        bool IsDefault,
        bool EnableLateDeduction,
        LateDeductionType LateDeductionType,
        decimal DeductionValue
    ) : IRequest;
}
