using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public record ResetPasswordCommand(
        string Email,
        string Otp,
        string NewPassword
    ) : IRequest<bool>;
}
