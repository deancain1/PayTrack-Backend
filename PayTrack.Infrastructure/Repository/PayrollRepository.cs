using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
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
      

        public async Task AddAsync(Payrolls payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }

        public async Task<Payrolls?> GetByIdAsync(Guid id)
        {
            return await _context.Payrolls
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Payrolls payroll)
        {
            _context.Payrolls.Update(payroll);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            return await _context.Payrolls.AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.PeriodStart == periodStart &&
                x.PeriodEnd == periodEnd);
        }
        public async Task<List<Payrolls>> GetPayrollsByCompanyAndYearAsync(string companyAdminId, int year)
        {
            return await _context.Payrolls
                .Include(x => x.Employee)
                .Where(x =>
                    x.Employee.CompanyAdminId == companyAdminId &&
                    x.PayrollBatch.IsActive &&
                    x.PeriodEnd.Year == year)
                .ToListAsync();
        }

        public async Task<bool> HasGeneratedPayrollForDateAsync(Guid employeeId, DateTime requestDate)
        {
            return await _context.Payrolls.AnyAsync(p =>
                p.EmployeeId == employeeId &&
                requestDate >= p.PeriodStart &&
                requestDate <= p.PeriodEnd);
        }
    }
}
