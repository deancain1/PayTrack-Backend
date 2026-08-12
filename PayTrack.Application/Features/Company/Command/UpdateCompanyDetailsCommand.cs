using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Company.Command
{
    public record UpdateCompanyDetailsCommand(
        string CompanyName,
        string? CompanyEmail,
        string? CompanyContactNumber,
        string? CompanyAddress,
        string? BusinessType,
        string? CompanyLogo
    ) : IRequest<CompanyDetailsDTO>;
}
