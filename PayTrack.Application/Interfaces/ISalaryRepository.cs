using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface ISalaryRepository
    {
        Task<EmployeeSalary>GetByEmployeeIdAsync(Guid employeeId);
        Task AddAsync(EmployeeSalary salary);
        Task UpdateAsync(EmployeeSalary salary);
        IQueryable<EmployeeSalary> GetEmployeeSalaryQueryByCompanyAdmin(string companyAdminId);
        Task<List<EmployeeSalary>> GetAllAsync(string companyAdminId);
        Task<decimal> GetDailyRateAsync(Guid employeeId);
        Task<List<EmployeeSalary>> GetByIdsAsync(List<Guid> ids);
        Task DeleteRangeAsync(List<EmployeeSalary> salaries);
    }
}
