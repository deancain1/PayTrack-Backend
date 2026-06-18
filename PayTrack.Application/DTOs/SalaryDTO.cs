using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalaryDTO(
       Guid EmployeeId,
       string? FullName,
       string EmployeeNumber,
       string Department,
       decimal Rate,
       int RequiredHours,
       string? ProfilePictureBase64,
       PayrollPolicy Policy
   );
}

