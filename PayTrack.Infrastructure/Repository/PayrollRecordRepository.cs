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
    public class PayrollRecordRepository : IPayrollRecordRepository
    {
        private readonly AppDbContext _context;
        public PayrollRecordRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PayrollRecord payroll)
        {
            await _context.PayrollRecords.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> GetTotalEarningsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            return await _context.PayrollRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.CreatedAt.Date >= periodStart.Date &&
                    x.CreatedAt.Date <= periodEnd.Date)
                .SumAsync(x => x.AmountEarned);
        }

        public IQueryable<PayrollRecord> Query()
        {
            return _context.PayrollRecords.AsQueryable();
        }
    }
}
