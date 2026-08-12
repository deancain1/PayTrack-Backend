using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IAttendancePolicyRepository
    {
        Task<List<AttendancePolicy>> GetAllByCompanyAsync(string companyAdminId);

        Task<AttendancePolicy?> GetByIdAsync(Guid id);

        Task<AttendancePolicy?> GetDefaultPolicyAsync(string companyAdminId);

        Task AddAsync(AttendancePolicy policy);

        Task UpdateAsync(AttendancePolicy policy);

        Task DeleteAsync(AttendancePolicy policy);
    }
}

