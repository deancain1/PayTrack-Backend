using Microsoft.EntityFrameworkCore;
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
    public class SalaryAdvanceRepository : BaseRepository<SalaryAdvances>, ISalaryAdvanceRepository
    {

        public SalaryAdvanceRepository(AppDbContext context) : base(context) { }

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
        public async Task<decimal> GetTotalUndeductedAdvancesAsync(Guid emploeeId)
        {
            return await _context.SalaryAdvances
                .Where(x =>
                    x.EmployeeId == emploeeId &&
                     x.Status == SalaryAdvanceStatus.Approved &&
                    !x.IsDeducted)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<decimal> GetTotalAdvancesAsync(string employeeApplicationUserId, DateTime startDate, DateTime endDate)
        {
            startDate = DateTime.SpecifyKind(
                startDate.Date,
                DateTimeKind.Utc);

            endDate = DateTime.SpecifyKind(
                endDate.Date.AddDays(1),
                DateTimeKind.Utc);

            return await _context.SalaryAdvances
                .Where(x =>
                    x.ApplicationUserId == employeeApplicationUserId &&
                    x.Status == SalaryAdvanceStatus.Approved &&
                    x.RequestDate >= startDate &&
                    x.RequestDate < endDate)
                .SumAsync(x => x.Amount);
        }

        public IQueryable<SalaryAdvances> GetSalaryAdvancesbyEmployee(string employeeApplicationUserId)
        {
            return _context.SalaryAdvances
                 .Include(s => s.Employee)
                 .ThenInclude(e => e.ApplicationUser)
                 .Where(e => e.ApplicationUserId == employeeApplicationUserId)
                 .AsQueryable();
        }

    }
}
