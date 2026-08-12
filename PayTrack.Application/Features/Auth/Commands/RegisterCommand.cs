using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public record RegisterCommand (
        string FullName,
        string? CompanyName,
        string? BusinessType,
        string Email,
        string? PhoneNumber,
        string Password,
        string Role
    ) : IRequest<RegisterCompanyAdminResponseDTO>;

}
