using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Features.AttendancePolicies.Queries;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Data;


namespace PayTrack.Infrastructure.Repository
{
    public class PayrollRecordRepository : BaseRepository<PayrollRecord>, IPayrollRecordRepository
    {
        public PayrollRecordRepository(AppDbContext context) : base(context) { }

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

        public async Task<bool> HasUnpaidAttendanceBeforePeriodAsync(
     DateTime periodStart)
        {
            return await _context.PayrollRecords
                .Include(r => r.Attendance)
                .Where(r => r.Attendance.Date < periodStart)
                .Where(r => r.AmountEarned > 0)
                .AnyAsync(r => r.PayrollId == null);
        }

        public async Task<List<PayrollRecord>> GetUnassignedPayrollRecordsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            return await _context.PayrollRecords
                .Include(r => r.Attendance)
                .Where(r => r.EmployeeId == employeeId)
                .Where(r => r.Attendance.Date >= periodStart)
                .Where(r => r.Attendance.Date <= periodEnd)
                .Where(r => r.AmountEarned > 0)
                .Where(r => r.PayrollId == null)
                .ToListAsync();
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

        public async Task<decimal> GetTotalUnpaidEarningsAsync(string employeeApplicationUserId)
        {
            return await _context.PayrollRecords
                .Where(x =>
                    x.ApplicationUserId == employeeApplicationUserId &&
                     x.Status == PayrollRecordStatus.Unpaid)
                .SumAsync(x => x.AmountEarned);
        }

        public async Task MarkRecordsAsPaidAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            await _context.PayrollRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Status == PayrollRecordStatus.Unpaid &&
                    x.CreatedAt.Date >= periodStart.Date &&
                    x.CreatedAt.Date <= periodEnd.Date)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        x => x.Status,
                        PayrollRecordStatus.Paid));
        }
    }
}
