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
    public class PayrollRepository : BaseRepository<Payrolls>, IPayrollRepository
    {
        
        public PayrollRepository(AppDbContext context) : base(context) { }

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
        public async Task<List<Payrolls>> GetEmployeePayrollsAsync(
           Guid employeeId)
        {
            return await _context.Payrolls
                .Include(x => x.Employee)
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Status != PayrollStatus.Pending)
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }
    }
}
