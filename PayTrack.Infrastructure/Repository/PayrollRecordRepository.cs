using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Features.AttendancePolicies.Queries;
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
    public class PayrollRecordRepository : IPayrollRecordRepository
    {
        private readonly AppDbContext _context;
        public PayrollRecordRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PayrollRecord payroll)
        {
            await _context.PayrollRecords.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> GetTotalEarningsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            return await _context.PayrollRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.CreatedAt.Date >= periodStart.Date &&
                    x.CreatedAt.Date <= periodEnd.Date)
                .SumAsync(x => x.AmountEarned);
        }

        public IQueryable<PayrollRecord> Query()
        {
            return _context.PayrollRecords.AsQueryable();
        }

        //Get Current Month earnings
        public async Task<decimal> GetTotalEarningsByEmployeeAsync(Guid employeeId)
        {
            var today = DateTime.Today;

            return await _context.PayrollRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.CreatedAt.Year == today.Year &&
                    x.CreatedAt.Month == today.Month)
                .SumAsync(x => (decimal?)x.AmountEarned) ?? 0;
        }

        public IQueryable<PayrollRecord> GetPayrollRecordByEmployeeAsync(string employeeApplicationUserId)
        {
           return _context.PayrollRecords
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == employeeApplicationUserId)
                .AsQueryable();
        }

        public async Task<bool> HasUnpaidAttendanceBeforePeriodAsync(DateTime periodStart)
        {
            return await _context.Attendances
                .Where(a => a.Date < periodStart)
                .AnyAsync(a =>
                    !_context.PayrollRecords
                    .Any(p => p.AttendanceId == a.Id)
                    );
        }

        public IQueryable<PayrollRecord> GetPayrollRecordByCompanyAdminsync(string companyAdminId)
        {
            return _context.PayrollRecords
                .Include(e => e.ApplicationUser)
                .Where(e => e.Employee.CompanyAdminId == companyAdminId)
                .AsQueryable();
        }

        public async Task<decimal> GetTotalPayrollTodayAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.PayrollRecords
                .Where(x =>
                    x.Employee.CompanyAdminId == companyAdminId &&
                    x.Attendance.TimeIn >= today &&
                    x.Attendance.TimeIn < tomorrow)
                .SumAsync(x => x.AmountEarned);
        }

        public async Task<decimal> GetAverageDailyPayoutAsync(string companyAdminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var query = _context.PayrollRecords
                .Where(x =>
                    x.Employee.CompanyAdminId == companyAdminId &&
                    x.Attendance.TimeIn >= today &&
                    x.Attendance.TimeIn < tomorrow);

            if (!await query.AnyAsync())
                return 0;

            return await query.AverageAsync(
                x => x.AmountEarned);
        }
    }
}
