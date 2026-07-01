using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record AttendancePolicyDTO(
       Guid Id,
       string Name,
       bool IsDefault,
       bool EnableLateDeduction,
       LateDeductionType LateDeductionType,
       decimal DeductionValue
   );
}
