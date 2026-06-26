using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IWorkScheduleRepository
    {
        Task<Guid> CreateAsync(WorkSchedules schedule);

        Task UpdateAsync(WorkSchedules schedule);

        Task DeleteAsync(Guid id);

        Task<WorkSchedules?> GetByIdAsync(Guid id);

        //Task<List<WorkSchedules>> GetAllByAdminAsync(string companyAdminId);

        Task AssignScheduleToEmployeeAsync(Guid employeeId,Guid scheduleId);
        IQueryable<WorkSchedules> GetSchedulesByCompanyAdminQuery(string companyAdminId);
        IQueryable<WorkSchedules> GetWorkSchedulesQuery(string companyAdminId);
    }
}

