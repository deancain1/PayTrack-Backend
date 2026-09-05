using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Data;

namespace PayTrack.Infrastructure.Repository
{
    public class LeaveRequestRepository : BaseRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<LeaveRequest> GetLeavesByEmployeeId(Guid employeeId)
        {
            return _context.LeaveRequests
                .AsNoTracking()
                .Where(lr => lr.EmployeeId == employeeId);
        }

        public IQueryable<LeaveRequest> GetLeaveRequestsByCompanyAdmin(string companyAdminId)
        {
            return _context.LeaveRequests
                .AsNoTracking()
                .Include(lr => lr.Employee)
                    .ThenInclude(e => e.ApplicationUser)
                .Include(lr => lr.Employee)
                    .ThenInclude(e => e.Department)
                .Where(lr => lr.Employee.CompanyAdminId == companyAdminId);
        }

        public async Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
          
            startDate = startDate.Kind == DateTimeKind.Utc
                ? startDate
                : startDate.ToUniversalTime();

            endDate = endDate.Kind == DateTimeKind.Utc
                ? endDate
                : endDate.ToUniversalTime();

            return await _context.LeaveRequests
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Status != LeaveStatus.Rejected &&
                    x.Status != LeaveStatus.Cancelled &&
                    startDate <= x.EndDate &&
                    endDate >= x.StartDate);
        }

        public async Task<LeaveRequest?> GetLeavesWithEmployeeByIdAsync(Guid id)
        {
            return await _context.LeaveRequests
                .Include(x => x.Employee)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Employee)
                    .ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(int Total, int Approved, int Rejected)> GetLeaveRequestCountsAsync(string companyAdminId)
        {
            var query = _context.LeaveRequests
                .Where(x => x.CompanyAdminId == companyAdminId);

            var total = await query.CountAsync();

            var approved = await query.CountAsync(
                x => x.Status == LeaveStatus.Approved);

            var rejected = await query.CountAsync(
                x => x.Status == LeaveStatus.Rejected);

            return (total, approved, rejected);
        }

        public async Task<List<LeaveRequest>> GetApprovedPaidLeaveAsync(Guid employeeId, DateTime startDate, DateTime endDate)
        {
            
            startDate = startDate.Kind == DateTimeKind.Utc
                ? startDate
                : startDate.ToUniversalTime();

            endDate = endDate.Kind == DateTimeKind.Utc
                ? endDate
                : endDate.ToUniversalTime();

            return await _context.LeaveRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Status == LeaveStatus.Approved &&
                    x.IsPaid &&
                    x.StartDate <= endDate &&
                    x.EndDate >= startDate)
                .ToListAsync();
        }
    }
}