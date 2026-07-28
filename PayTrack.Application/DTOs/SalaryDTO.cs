using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalaryDTO(
       Guid Id,
       Guid EmployeeId,
       string? FullName,
       string EmployeeNumber,
       string Department,
       decimal Rate,
       string? ProfilePictureBase64,
       PayrollPolicy Policy
   );
}

