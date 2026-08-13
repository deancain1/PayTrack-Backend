using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IEmployeesRepository : IBaseRepository<Employees>
    {
        IQueryable<Employees> GetEmployeesWithApplicationUser();
        Task<List<Employees>> GetAllEmployeesByCompanyAdminAsync(string companyAdminId);
        IQueryable<Employees> GetEmployeesByCompanyAdminQuery(string companyAdminId);
        Task<Employees?> GetByIdAndNumberAsync(Guid employeeId, string employeeNumber);
        Task<Employees?> GetCurrentEmployeeByApplicationUserIdAsync(string employeeApplicationUserId);
        Task<List<Employees>> GetByIdsAsync(List<Guid> ids);
        Task<List<Employees>> GetAllAsync();
        Task<int> GetTotalEmployeesByCompanyAdminAsync(string companyAdminId);
        Task<List<Employees>> GetEmployeesForAbsentAutomationAsync();
        Task<List<Employees>> GetEmployeesWithAssignedScheduleAsync();
        Task<bool> HasEmployeesInDepartmentsAsync(List<Guid> departmentIds);

    }
}
