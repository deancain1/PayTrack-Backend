using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeePayslipDTO(
       Guid Id,
       Guid EmployeeId,
       string EmployeeName,

       DateTime PeriodStart,
       DateTime PeriodEnd,

       decimal RegularAmount,
       decimal OvertimeAmount,
       decimal TotalAmount,

       decimal SalaryAdvance,
       decimal PhilHealthDeduction,
       decimal PagIbigDeduction,
       decimal SssDeduction,

       decimal NetAmount,

       PayrollStatus Status,

       DateTime? ReleasedAt,
       DateTime? PaidAt
   );
}
