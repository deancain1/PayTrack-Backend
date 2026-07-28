using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Departments>
    {
        IQueryable<Departments> GetDepartmentsQuery();
        Task<List<Departments>> GetDepartmentsByUserIdAsync(string userId);
        Task<List<Departments>> GetByIdsAsync(List<Guid> ids);
        Task DeleteRangeAsync(List<Departments> departments);
    }
}
