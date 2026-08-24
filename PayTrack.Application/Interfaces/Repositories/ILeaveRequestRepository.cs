using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface  ILeaveRequestRepository : IBaseRepository<LeaveRequest>
    {

        IQueryable<LeaveRequest> GetLeavesByEmployeeId(Guid employeeId);
        IQueryable<LeaveRequest> GetLeaveRequestsByCompanyAdmin(string companyAdminId);
        Task<LeaveRequest?> GetLeavesWithEmployeeByIdAsync(Guid id);
        Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateTime startDate, DateTime endDate);
        Task<(int Total, int Approved, int Rejected)>GetLeaveRequestCountsAsync(string companyAdminId);
        Task<List<LeaveRequest>> GetApprovedPaidLeaveAsync(Guid employeeId, DateTime startDate, DateTime endDate);
    }
}
