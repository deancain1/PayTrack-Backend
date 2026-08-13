using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeePayrollDTO(
     Guid EmployeeId,
     string EmployeeName,
     decimal TotalAmount,
     decimal SalaryAdvance,
     decimal PhilHealthDeduction,
     decimal PagIbigDeduction,
     decimal SssDeduction,
     decimal NetAmount
 );
}
