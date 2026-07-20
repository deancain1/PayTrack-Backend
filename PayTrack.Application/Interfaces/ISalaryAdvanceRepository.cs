using Microsoft.EntityFrameworkCore;
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
        // Get the total undeducted advances for a specific employee
        Task<decimal> GetTotalUndeductedAdvancesAsync(Guid employeeId);
        Task<bool> HasPendingRequestAsync(Guid employeeId);
        IQueryable<SalaryAdvances> GetPendingSalaryAdvanceRequestsQueryByCompanyAdmin(string companyAdminId);
        Task<SalaryAdvances?> GetByIdAsync(Guid id);
        Task UpdateAsync(SalaryAdvances salaryAdvance);
    }
}
