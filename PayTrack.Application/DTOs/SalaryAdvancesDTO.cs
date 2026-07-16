using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalaryAdvancesDTO
    (
        Guid Id,
        string fullName,
        decimal Amount,
        string Remarks,
        DateTime RequestDate

   );
}
