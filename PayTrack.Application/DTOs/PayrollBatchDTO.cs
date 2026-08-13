using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record PayrollBatchDTO(
       Guid Id,
       DateTime PeriodStart,
       DateTime PeriodEnd,
       PayrollBatchStatus Status,
       int EmployeeCount,
       decimal TotalPayroll
   );
}
