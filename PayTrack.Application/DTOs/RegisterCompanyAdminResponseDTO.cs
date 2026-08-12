using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record RegisterCompanyAdminResponseDTO(
      string FullName,
      string PhoneNumber,
      string Email,
      string UserId,
      string Role,
      Guid CompanyId
  );
}
