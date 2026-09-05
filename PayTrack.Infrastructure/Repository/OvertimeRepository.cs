using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Data;

namespace PayTrack.Infrastructure.Repository
{
    public class OvertimeRepository
        : BaseRepository<OvertimeRequest>,
          IOvertimeRepository
    {
        public OvertimeRepository(AppDbContext context) : base(context){}

        public async Task<OvertimeRequest?> GetByIdWithApplicationUserAsync(Guid id)
        {
            return await _context.OvertimeRequests
                .Include(x => x.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<List<OvertimeRequest>> GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            // Normalize the payroll period to UTC
            var startDate = DateTime.SpecifyKind(
                periodStart.Date,
                DateTimeKind.Utc);

            var endDate = DateTime.SpecifyKind(
                periodEnd.Date.AddDays(1),
                DateTimeKind.Utc);

            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= startDate &&
                    x.Date < endDate &&
                    x.Status == OvertimeStatus.Completed &&
                    x.PayrollRecordId == null)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(Guid employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeId)
        {
            // Normalize date to UTC
            var overtimeDate = DateTime.SpecifyKind(
                date.Date,
                DateTimeKind.Utc);

            var nextDate = overtimeDate.AddDays(1);

            return await _context.OvertimeRequests
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&

                    x.Date >= overtimeDate &&
                    x.Date < nextDate &&

                    x.Id != excludeId &&

                    x.Status != OvertimeStatus.Rejected &&
                    x.Status != OvertimeStatus.Cancelled &&

                    startTime < x.EndTime &&
                    endTime > x.StartTime);
        }

        public IQueryable<OvertimeRequest>GetOvertimeQueryByCompanyAdmin(string companyAdminId)
        {
            return _context.OvertimeRequests
                .Include(o => o.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Include(o => o.Employee)
                    .ThenInclude(e => e.Department)
                .Where(o =>
                    o.Employee.CompanyAdminId == companyAdminId);
        }

        public IQueryable<OvertimeRequest>GetOvertimeQueryByEmployee(string employeeApplicationUserId)
        {
            return _context.OvertimeRequests
                .Include(e => e.Employee)
                .Where(e =>
                    e.Employee.ApplicationUserId ==
                        employeeApplicationUserId &&
                    e.Source == OvertimeSource.CompanyAdmin)
                .AsQueryable();
        }

        public async Task<OvertimeRequest?> GetActiveOvertimeForEmployeeAsync(Guid employeeId, DateTime date)
        {
            // Normalize date to UTC
            var overtimeDate = DateTime.SpecifyKind(
                date.Date,
                DateTimeKind.Utc);

            var nextDate = overtimeDate.AddDays(1);

            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&

                    x.Date >= overtimeDate &&
                    x.Date < nextDate &&

                    x.Status == OvertimeStatus.Accepted)
                .OrderBy(x => x.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<OvertimeRequest>>GetCompletedOvertimesForPayrollAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            // Normalize payroll dates to UTC
            var start = DateTime.SpecifyKind(
                startDate.Date,
                DateTimeKind.Utc);

            var end = DateTime.SpecifyKind(
                endDate.Date.AddDays(1),
                DateTimeKind.Utc);

            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&

                    x.Date >= start &&
                    x.Date < end &&

                    x.Status == OvertimeStatus.Completed &&
                    x.PayrollRecordId == null)
                .ToListAsync();
        }

        public async Task<OvertimeRequest?>GetAcceptedOvertimeAsync(Guid employeeId, DateTime date)
        {
            
            var overtimeDate = DateTime.SpecifyKind(
                date.Date,
                DateTimeKind.Utc);

            var nextDate = overtimeDate.AddDays(1);

            return await _context.OvertimeRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&

                    x.Date >= overtimeDate &&
                    x.Date < nextDate &&

                    x.Status == OvertimeStatus.Accepted)
                .OrderByDescending(x => x.EndTime)
                .FirstOrDefaultAsync();
        }
    }
}