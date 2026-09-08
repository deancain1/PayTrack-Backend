using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;

namespace PayTrack.Infrastructure.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attendances attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Attendances> GetAttendancesByEmployeeId(Guid employeeId)
        {
            return _context.Attendances
                .AsNoTracking()
                .Where(a => a.EmployeeId == employeeId);
        }

        public IQueryable<Attendances> GetAttendancesByCompanyAdminQuery(string companyAdminId)
        {
            return _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Where(a => a.Employee.CompanyAdminId == companyAdminId)
                .OrderByDescending(a => a.TimeIn);
        }

        public async Task<List<Attendances>> GetAttendanceByMonthAsync(int month, int year)
        {
            var startDate = new DateTime(
                year,
                month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var endDate = startDate.AddMonths(1);

            return await _context.Attendances
                .Where(x =>
                    x.TimeIn.HasValue &&
                    x.TimeIn.Value >= startDate &&
                    x.TimeIn.Value < endDate)
                .ToListAsync();
        }

        public Task<Attendances?> GetTodayAttendanceAsync(Guid employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return _context.Attendances
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= today &&
                    x.Date < tomorrow);
        }

        public async Task<List<Attendances>> GetTodayAttendancesAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a =>
                    a.Date >= today &&
                    a.Date < tomorrow &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.TimeIn != null)
                .OrderByDescending(a => a.TimeIn)
                .ToListAsync();
        }

        public async Task<int> GetPresentTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .Where(a =>
                    a.Date >= today &&
                    a.Date < tomorrow &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Present")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetLateTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .Where(a =>
                    a.Date >= today &&
                    a.Date < tomorrow &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Late")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetAbsentTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .Where(a =>
                    a.Date >= today &&
                    a.Date < tomorrow &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Absent")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }

        public async Task<(int Present, int Late, int Absent)>GetAttendanceSummaryAsync(
                string companyAdminId,
                int month,
                int year)
        {
            month = month <= 0
                ? DateTime.UtcNow.Month
                : month;

            year = year <= 0
                ? DateTime.UtcNow.Year
                : year;

            var startDate = new DateTime(
                year,
                month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var endDate = startDate.AddMonths(1);

            var attendances = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a =>
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Date >= startDate &&
                    a.Date < endDate)
                .ToListAsync();

            return (
                attendances.Count(x => x.Status == "Present"),
                attendances.Count(x => x.Status == "Late"),
                attendances.Count(x => x.Status == "Absent")
            );
        }

        public async Task UpdateAsync(Attendances attendance)
        {
            _context.Attendances.Update(attendance);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AttendanceExistsTodayAsync(Guid employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= today &&
                    x.Date < tomorrow);
        }

        public async Task<List<Attendances>> GetByEmployeeAndDateRangeAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            
            startDate = startDate.Kind == DateTimeKind.Utc
                ? startDate
                : startDate.ToUniversalTime();

            endDate = endDate.Kind == DateTimeKind.Utc
                ? endDate
                : endDate.ToUniversalTime();

            return await _context.Attendances
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.Date >= startDate &&
                    a.Date <= endDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveTrackedAsync(string companyAdminId)
        {
            return await _context.Attendances
                .CountAsync(x =>
                    x.Employee.CompanyAdminId == companyAdminId &&
                    x.TimeIn != null &&
                    x.TimeOut == null);
        }
        public async Task<decimal> GetHoursWorkedForEmployeePeriodAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.Date >= startDate &&
                    a.Date < endDate)
                .SumAsync(
                    a => a.HoursWorked);
        }
    }
}