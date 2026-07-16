using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface ISalaryAdvanceRepository
    {
        Task<decimal> GetTotalAdvancesAsync(Guid employeeId, DateTime startDate, DateTime endDate);
        Task AddAsync(SalaryAdvances salaryAdvance);
        IQueryable<SalaryAdvances> GetSalaryAdvancesbyEmployee(string employeeApplicationUserId);
    }
}
