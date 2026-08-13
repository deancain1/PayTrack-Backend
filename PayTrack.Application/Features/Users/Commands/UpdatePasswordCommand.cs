using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Users.Commands
{
    public record UpdatePasswordCommand
    (
        string currentPassword,
        string newPassword
    ) :IRequest<bool>;
}
