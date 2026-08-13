using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeeDTO(
      Guid Id,
      string ApplicationUserId,
      string FullName,
      string? Email,
      string EmployeeNumber,
      string Department,
      string? PhoneNumber,
      string? ProfilePictureBase64,
      string? QrCodeBase64

  );
}
