using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface  IOvertimeRepository : IBaseRepository<OvertimeRequest>
    {

        Task<OvertimeRequest?> GetByIdWithApplicationUserAsync(Guid id);
        Task<List<OvertimeRequest>> GetByEmployeeAsync(Guid employeeId);
        Task<List<OvertimeRequest>> GetByCompanyAdminAsync(string companyAdminId);
        Task<List<OvertimeRequest>> GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd);
        Task<bool> HasOverlapAsync(Guid employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeId);
        IQueryable<OvertimeRequest> GetOvertimeQueryByCompanyAdmin( string companyAdminId);
        Task<OvertimeRequest?> GetActiveOvertimeForEmployeeAsync(Guid employeeId,DateTime date);
        Task<List<OvertimeRequest>> GetCompletedOvertimesForPayrollAsync(Guid employeeId,DateTime startDate,DateTime endDate);
    }
}

