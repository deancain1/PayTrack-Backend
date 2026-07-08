using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeeLookupDTO
    (
      Guid Id,
      string ApplicationUserId,
      string FullName,
      string EmployeeNumber,
      string Department
    );
}
