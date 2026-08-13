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

    public class AttendancePolicyRepository : BaseRepository<AttendancePolicy>, IAttendancePolicyRepository
    {
        public AttendancePolicyRepository(AppDbContext context) : base(context) { }
        public async Task<List<AttendancePolicy>> GetAllByCompanyAsync(string companyAdminId)
        {
            return await _context.AttendancePolicies
                .Where(x => x.CompanyAdminId == companyAdminId)
                .ToListAsync();
        }

        public async Task<AttendancePolicy?> GetDefaultPolicyAsync(string companyAdminId)
        {
            return await _context.AttendancePolicies
                .FirstOrDefaultAsync(x =>
                    x.CompanyAdminId == companyAdminId &&
                    x.IsDefault);
        }

    }
}

