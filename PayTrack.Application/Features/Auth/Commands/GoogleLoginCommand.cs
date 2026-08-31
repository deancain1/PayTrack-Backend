using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public record GoogleLoginCommand(
      string Provider,
      string ProviderKey,
      string Email,
      string FullName
  ) : IRequest<AuthResponseDTO>;

}
