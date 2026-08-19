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

        public async Task<PayrollBatch?> GetOverlappingActiveBatchAsync(
     DateTime periodStart,
     DateTime periodEnd)
        {
            return await _context.PayrollBatches
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
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