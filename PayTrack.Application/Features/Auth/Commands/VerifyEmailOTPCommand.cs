using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public record VerifyEmailOtpCommand(
      string UserId,
      string Otp
  ) : IRequest<AuthResponseDTO>;
}
