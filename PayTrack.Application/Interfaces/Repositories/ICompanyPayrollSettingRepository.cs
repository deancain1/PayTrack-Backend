using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface ICompanyPayrollSettingRepository
    {
        Task<CompanyPayrollSetting?> GetByCompanyAdminIdAsync(string companyAdminId);
        Task AddAsync(CompanyPayrollSetting setting);
        Task UpdateAsync(CompanyPayrollSetting setting);
    }
}
