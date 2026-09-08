using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record ScanEmployeeForAdvanceDTO(
        Guid EmployeeId,
        string ApplicationUserId,
        string EmployeeNumber,
        string EmployeeName,
        string Department,
        decimal CurrentEarnings,
        decimal AlreadyAdvanced,
        decimal AvailableAdvance
    );
}
