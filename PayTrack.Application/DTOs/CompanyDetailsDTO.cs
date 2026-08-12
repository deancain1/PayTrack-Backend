using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record CompanyDetailsDTO(
        Guid Id,
        string CompanyName,
        string? CompanyEmail,
        string? CompanyContactNumber,
        string? CompanyAddress,
        string? BusinessType,
        string? CompanyLogo
    );
}
