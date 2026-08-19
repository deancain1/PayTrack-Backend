using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IPayrollRecordRepository : IBaseRepository<PayrollRecord>
    {
        Task<decimal> GetTotalEarningsAsync( Guid employeeId, DateTime periodStart, DateTime periodEnd);
        IQueryable<PayrollRecord> Query();
        IQueryable<PayrollRecord> GetPayrollRecordByEmployeeAsync(string employeeApplicationUserId);
        IQueryable<PayrollRecord> GetPayrollRecordByCompanyAdminsync(string companyAdminId);
        Task<decimal> GetTotalEarningsByEmployeeAsync(Guid employeeId);
        Task<bool> HasUnpaidAttendanceBeforePeriodAsync(DateTime periodStart);
        Task<List<PayrollRecord>> GetUnassignedPayrollRecordsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd);
        Task<decimal> GetTotalPayrollTodayAsync(string companyAdminId);

        Task<decimal> GetAverageDailyPayoutAsync(string companyAdminId);

        Task<decimal> GetTotalUnpaidEarningsAsync(string employeeApplicationUserId);
        Task MarkRecordsAsPaidAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd);
    }
}
