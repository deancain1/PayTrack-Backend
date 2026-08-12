using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IPayrollBatchRepository
    {
        Task<PayrollBatch?> GetByIdAsync(Guid id);
        Task<PayrollBatch?> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd);
        Task AddAsync(PayrollBatch batch);
        Task UpdateAsync(PayrollBatch batch);
        Task<bool> HasOverlappingBatchAsync(DateTime periodStart, DateTime periodEnd, Guid? excludeBatchId);
        Task<PayrollBatch?> GetByIdWithPayrollsAsync(Guid id);

        Task<PayrollBatch?> GetActiveBatchAsync(string companyAdminId);
    }
}
