using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces;
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
    public class SalaryAdvanceRepository : ISalaryAdvanceRepository
    {
        private readonly AppDbContext _context;
        public SalaryAdvanceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> HasPendingRequestAsync(Guid employeeId)
        {
            return await _context.SalaryAdvances.AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.Status == SalaryAdvanceStatus.Pending);
        }
        public IQueryable<SalaryAdvances> GetPendingSalaryAdvanceRequestsQueryByCompanyAdmin(string companyAdminId)
        {
            return _context.SalaryAdvances
                .Include(x => x.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Where(x =>
                    x.Employee.CompanyAdminId == companyAdminId &&
                    x.Status == SalaryAdvanceStatus.Pending)
                .AsNoTracking();
        }
        public async Task<decimal> GetTotalUndeductedAdvancesAsync(Guid employeeId)
        {
            return await _context.SalaryAdvances
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    !x.IsDeducted)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<decimal> GetTotalAdvancesAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            return await _context.SalaryAdvances
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Status == SalaryAdvanceStatus.Approved &&
                    x.RequestDate >= startDate &&
                    x.RequestDate <= endDate)
                .SumAsync(x => x.Amount);
        }
        public async Task AddAsync(SalaryAdvances salaryAdvance)
        {
            await _context.SalaryAdvances.AddAsync(salaryAdvance);

            await _context.SaveChangesAsync();
        }

        public IQueryable<SalaryAdvances> GetSalaryAdvancesbyEmployee(string employeeApplicationUserId)
        {
            return _context.SalaryAdvances
                 .Include(s => s.Employee)
                 .ThenInclude(e => e.ApplicationUser)
                 .Where(e => e.ApplicationUserId == employeeApplicationUserId)
                 .AsQueryable();
        }

        public async Task<SalaryAdvances?> GetByIdAsync(Guid id)
        {
            return await _context.SalaryAdvances
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);
        }



        public async Task UpdateAsync(
            SalaryAdvances salaryAdvance)
        {
            _context.SalaryAdvances.Update(salaryAdvance);

            await _context.SaveChangesAsync();
        }
    }
}
