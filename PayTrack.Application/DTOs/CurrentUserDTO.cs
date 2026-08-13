using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record CurrentUserDTO(
     string Id,
     string FullName,
     string Email,
     string Role,
     string? Department,
     string? ProfilePictureBase64
 );
}
