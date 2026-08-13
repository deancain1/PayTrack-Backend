using Microsoft.EntityFrameworkCore;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface ISalaryAdvanceRepository : IBaseRepository<SalaryAdvances>
    {
        Task<decimal> GetTotalAdvancesAsync(string employeeApplicationUserId, DateTime startDate, DateTime endDate);
        IQueryable<SalaryAdvances> GetSalaryAdvancesbyEmployee(string employeeApplicationUserId);
        // Get the total undeducted advances for a specific employee
        Task<decimal> GetTotalUndeductedAdvancesAsync(Guid employeeId);
        Task<bool> HasPendingRequestAsync(Guid employeeId);
        IQueryable<SalaryAdvances> GetPendingSalaryAdvanceRequestsQueryByCompanyAdmin(string companyAdminId);

    }
}
