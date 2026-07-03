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
        public IQueryable<Attendances> GetAttendancesByCompanyAdminQuery(string companyAdminId)
        {
            return _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Where(a => a.Employee.CompanyAdminId == companyAdminId);
        }

        public Task<Attendances?> GetTodayAttendanceAsync(Guid employeeId)
        {
           var today = DateTime.UtcNow.Date;
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
                .Where(a =>
                    a.Date.Date == today &&
                    a.Employee.  CompanyAdminId == companyAdminId && a.TimeIn != null)
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
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var attendances = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a =>
                    a.Employee.CompanyAdminId == companyAdminId &&
                    a.Date >= startDate &&
                    a.Date < endDate)
                .ToListAsync();

            var present = attendances.Count(x => x.Status == "Present");
            var late = attendances.Count(x => x.Status == "Late");
            var absent = attendances.Count(x => x.Status == "Absent");

            return (present, late, absent);
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

     
    }
    
}
