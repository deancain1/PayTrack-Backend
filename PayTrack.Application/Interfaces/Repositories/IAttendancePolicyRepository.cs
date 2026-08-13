using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IAttendancePolicyRepository : IBaseRepository<AttendancePolicy>
    {
        Task<List<AttendancePolicy>> GetAllByCompanyAsync(string companyAdminId);
        Task<AttendancePolicy?> GetDefaultPolicyAsync(string companyAdminId);

       
    }
}

