using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeeNotificationPreferenceDTO
      (
          bool SalaryAdvanceApproved,
          bool SalaryAdvanceRejected,
          bool PayrollGenerated,
          bool PayrollReleased
      );
}
