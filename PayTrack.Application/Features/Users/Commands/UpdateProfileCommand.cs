using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Users.Commands
{
   public record UpdateProfileCommand
   (
    string FullName,
    string Email,
    string? ProfilePicture
   ) : IRequest<Unit>;
}
