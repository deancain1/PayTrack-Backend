using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    internal class PayrollBatchRepository : IPayrollBatchRepository
    {
        private readonly AppDbContext _context;
        public PayrollBatchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PayrollBatch?> GetByIdAsync(Guid id)
        {
            return await _context.PayrollBatches
                .FirstOrDefaultAsync(x => x.Id == id);
        }

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

        public async Task AddAsync(PayrollBatch batch)
        {
            await _context.PayrollBatches.AddAsync(batch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PayrollBatch batch)
        {
            _context.PayrollBatches.Update(batch);
            await _context.SaveChangesAsync();
        }
    }

}