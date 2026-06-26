using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task AddAsync(Attendances attendance);
        Task<Attendances?> GetTodayAttendanceAsync(Guid employeeId);
        Task UpdateAsync(Attendances attendance);
        Task<List<Attendances>> GetTodayAttendancesAsync(string companyAdminId);
        Task<int> GetPresentTodayCountAsync(string companyAdminId);
        Task<int> GetLateTodayCountAsync(string companyAdminId);
        Task<int> GetAbsentTodayCountAsync(string companyAdminId);



    }
}

