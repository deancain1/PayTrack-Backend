using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record RegisterEmployeeResponseDTO(
      string FullName,
      string PhoneNumber,
      string Email,
      Guid DepartmentId,
      string EmployeeNumber,
      string UserId,
      string Role
  );
}
