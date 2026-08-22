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
    public class OvertimeRepository : BaseRepository<OvertimeRequest>, IOvertimeRepository
    {
        public OvertimeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<OvertimeRequest?> GetByIdWithApplicationUserAsync(Guid id)
        {
            return await _context.OvertimeRequests
                .Include(x => x.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .FirstOrDefaultAsync(
                    x => x.Id == id
                    );
        }

        public async Task<List<OvertimeRequest>> GetByEmployeeAsync(Guid employeeId)
        {
            return await _context.OvertimeRequests
                .Include(x => x.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<OvertimeRequest>> GetByCompanyAdminAsync(string companyAdminId)
        {
            return await _context.OvertimeRequests
                .Include(x => x.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Where(x =>
                    x.Employee.CompanyAdminId == companyAdminId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<OvertimeRequest>>GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date.Date >= periodStart.Date &&
                    x.Date.Date <= periodEnd.Date &&
                    x.Status == OvertimeStatus.Completed &&
                    x.PayrollRecordId == null)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(Guid employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeId)
        {
            return await _context.OvertimeRequests
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Date.Date == date.Date &&
                    x.Id != excludeId &&
                    x.Status != OvertimeStatus.Rejected &&
                    x.Status != OvertimeStatus.Cancelled &&
                    startTime < x.EndTime &&
                    endTime > x.StartTime
                    );
        }
        public IQueryable<OvertimeRequest> GetOvertimeQueryByCompanyAdmin(string companyAdminId)
        {
            return _context.OvertimeRequests

                .Include(o => o.Employee)
                    .ThenInclude(e => e.ApplicationUser)

                .Include(o => o.Employee)
                    .ThenInclude(e => e.Department)

                .Where(o =>
                    o.Employee.CompanyAdminId ==
                    companyAdminId
                );
        }
        public async Task<OvertimeRequest?> GetActiveOvertimeForEmployeeAsync(Guid employeeId, DateTime date)
        {
            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date.Date == date.Date &&
                    x.Status == OvertimeStatus.Accepted)
                .OrderBy(x => x.StartTime)
                .FirstOrDefaultAsync();
        }
      
        public async Task<List<OvertimeRequest>> GetCompletedOvertimesForPayrollAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date.Date >= startDate.Date &&
                    x.Date.Date <= endDate.Date &&
                    x.Status == OvertimeStatus.Completed &&
                    x.PayrollRecordId == null)
                .ToListAsync();
        }
    }
}
