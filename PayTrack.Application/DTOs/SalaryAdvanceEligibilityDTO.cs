using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record SalaryAdvanceEligibilityDTO(
      decimal GrossEarnings,
      decimal SSSDeduction,
      decimal PhilHealthDeduction,
      decimal PagIbigDeduction,
      decimal ExistingSalaryAdvances,
      decimal TotalStatutoryDeductions,
      decimal AvailableSalaryAdvance,
      bool CanRequest
  );
}
