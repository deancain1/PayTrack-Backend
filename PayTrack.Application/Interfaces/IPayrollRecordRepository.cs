using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IPayrollRecordRepository
    {
        Task AddAsync(PayrollRecord payroll);
        Task<decimal> GetTotalEarningsAsync(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd);
        IQueryable<PayrollRecord> Query();
        IQueryable<PayrollRecord> GetPayrollRecordByEmployeeAsync(string employeeApplicationUserId);
        Task<decimal> GetTotalEarningsByEmployeeAsync(Guid employeeId);

        Task<bool> HasUnpaidAttendanceBeforePeriodAsync(DateTime periodStart);
    }
}
