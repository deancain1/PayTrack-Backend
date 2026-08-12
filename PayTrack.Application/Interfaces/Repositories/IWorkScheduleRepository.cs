using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IWorkScheduleRepository : IBaseRepository<WorkSchedules>
    {
   
        //Task<List<WorkSchedules>> GetAllByAdminAsync(string companyAdminId);

        IQueryable<WorkSchedules> GetSchedulesByCompanyAdminQuery(string companyAdminId);
        IQueryable<WorkSchedules> GetWorkSchedulesQuery(string companyAdminId);
        Task<List<WorkSchedules>> GetByIdsAsync(List<Guid> ids);
        Task DeleteRangeAsync(List<WorkSchedules> schedules);
    }
}

