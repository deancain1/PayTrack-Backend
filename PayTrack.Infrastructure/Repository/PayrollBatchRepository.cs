using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    internal class PayrollBatchRepository : BaseRepository<PayrollBatch>, IPayrollBatchRepository
    {
        public PayrollBatchRepository(AppDbContext context) : base(context) { }

        public async Task<PayrollBatch?> GetByPeriodAsync(
            DateTime periodStart,
            DateTime periodEnd)
        {
            return await _context.PayrollBatches
                .FirstOrDefaultAsync(x =>
                    x.PeriodStart.Date == periodStart.Date &&
                    x.PeriodEnd.Date == periodEnd.Date &&
                    x.IsActive);
        }

        //for overlapping batch check, we need to check if there is any existing batch that overlaps with the given period
        public async Task<bool> HasOverlappingBatchAsync(DateTime periodStart, DateTime periodEnd, Guid? excludeBatchId)
        {
            return await _context.PayrollBatches
                .Where(x => x.IsActive)
                .Where(x => excludeBatchId == null || x.Id != excludeBatchId)
                .AnyAsync(x =>
                    x.PeriodStart <= periodEnd &&
                    x.PeriodEnd >= periodStart);
        }

        public async Task<PayrollBatch?> GetByIdWithPayrollsAsync(Guid id)
        {
            return await _context.PayrollBatches
                .Include(x => x.Payrolls)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PayrollBatch?> GetActiveBatchAsync(string companyAdminId)
        {
            return await _context.PayrollBatches
                .Include(x => x.Payrolls)
                    .ThenInclude(x => x.Employee)
                .Where(x =>
                    x.IsActive &&
                    x.Payrolls.Any(p => p.Employee.CompanyAdminId == companyAdminId))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }

}