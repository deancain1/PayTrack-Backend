using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalaryAdvanceRequestDTO
    (
    Guid Id,
    Guid EmployeeId,
    string EmployeeNumber,
    string FullName,
    decimal Amount,
    string Remarks,
    DateTime RequestDate
    );
}
