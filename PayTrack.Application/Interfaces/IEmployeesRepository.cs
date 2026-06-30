using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IEmployeesRepository
    {
        Task AddEmployeeAsync(Employees employee);
        IQueryable<Employees> GetEmployeesWithApplicationUser();
        Task<Employees?> GetEmployeeByEmployeeIdAsync(Guid employeeId);
        Task UpdateAsync(Employees employee);
        IQueryable<Employees> GetEmployeesQuery();
        IQueryable<Employees> GetEmployeesByCompanyAdminQuery(string companyAdminId);
        Task<Employees?> GetByIdAndNumberAsync(Guid employeeId, string employeeNumber);
        Task<Employees?> GetByIdAsync(Guid id);
        Task<List<Employees>> GetByIdsAsync(List<Guid> ids);
        Task<List<Employees>> GetAllAsync();
        Task<int> GetTotalEmployeesByCompanyAdminAsync(string companyAdminId);
        Task<List<Employees>> GetEmployeesForAbsentAutomationAsync();


    }
}
