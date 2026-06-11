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
    public class PayrollRepository : IPayrollRepository
    {
        private readonly AppDbContext _context;
        public PayrollRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PayrollRecord payroll)
        {
            await _context.PayrollRecords.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }
    }
}
