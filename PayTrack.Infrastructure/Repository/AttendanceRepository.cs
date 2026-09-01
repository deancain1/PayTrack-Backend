using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                  .Where(lr => lr.EmployeeId == employeeId);
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
            return await _context.Attendances
                .Where(x =>
                    x.TimeIn.HasValue &&
                    x.TimeIn.Value.Month == month &&
                    x.TimeIn.Value.Year == year)
                .ToListAsync();
        }

        public Task<Attendances?> GetTodayAttendanceAsync(Guid employeeId)
        {
            var today = DateTime.Now.Date;
            return _context.Attendances
                .FirstOrDefaultAsync(x =>
                 x.EmployeeId == employeeId &&
                 x.Date == today);
        }
        public async Task<List<Attendances>> GetTodayAttendancesAsync(string companyAdminId)
        {
            var today = DateTime.Today;

            return await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                    .Include(a => a.Employee)
                     .ThenInclude(e => e.Department)
                .Where(a =>
                    a.Date.Date == today &&
                    a.Employee.CompanyAdminId == companyAdminId && a.TimeIn != null)
                .OrderByDescending(a => a.TimeIn)
                .ToListAsync();
        }
        public async Task<int> GetPresentTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.Today;

            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a =>
                    a.Date.Date == today &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Present")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }
        public async Task<int> GetLateTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.Today;

            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a =>
                    a.Date.Date == today &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Late")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }
        public async Task<int> GetAbsentTodayCountAsync(string companyAdminId)
        {
            var today = DateTime.Today;

            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a =>
                    a.Date.Date == today &&
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Status == "Absent")
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }
        public async Task<(int Present, int Late, int Absent)> GetAttendanceSummaryAsync(string companyAdminId, int month, int year)
        {
            month = month <= 0 ? DateTime.Today.Month : month;
            year = year <= 0 ? DateTime.Today.Year : year;

            var startDate = new DateTime(year, month, 1);
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
            var today = DateTime.Today;

            return await _context.Attendances.AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.Date == today);
        }



        public async Task<List<Attendances>> GetByEmployeeAndDateRangeAsync(Guid employeeId, DateTime startDate,DateTime endDate)
        {
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


    }

}