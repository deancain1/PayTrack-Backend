using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record AuthResponseDTO(
         string FullName,
         string PhoneNumber,
         string Email,
         string Token,
         string RefreshToken,
         string UserId,
         string Role,
         bool RequiresTwoFactor
     );
}
