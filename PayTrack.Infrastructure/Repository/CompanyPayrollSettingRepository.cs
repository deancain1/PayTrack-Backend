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
    public class CompanyPayrollSettingRepository: BaseRepository<CompanyPayrollSetting>, ICompanyPayrollSettingRepository
    {
        public CompanyPayrollSettingRepository(AppDbContext context) : base(context) { }
    
        public async Task<CompanyPayrollSetting?> GetByCompanyAdminIdAsync(string companyAdminId)
        {
            return await _context.CompanyPayrollSettings
                .FirstOrDefaultAsync(x =>
                    x.CompanyAdminId == companyAdminId);
        }
    }
}
