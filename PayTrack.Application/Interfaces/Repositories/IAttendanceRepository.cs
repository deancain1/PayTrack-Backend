using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IAttendanceRepository
    {
        Task AddAsync(Attendances attendance);
        IQueryable<Attendances> GetAttendancesByEmployeeId(Guid employeeId);
        IQueryable<Attendances> GetAttendancesByCompanyAdminQuery(string companyAdminId);
        Task<Attendances?> GetTodayAttendanceAsync(Guid employeeId);
        Task<(int Present, int Late, int Absent)> GetAttendanceSummaryAsync(string companyAdminId, int month, int year);
        Task UpdateAsync(Attendances attendance);

        //this must be iquerable for pagination and filtering
        Task<List<Attendances>> GetTodayAttendancesAsync(string companyAdminId);
        Task<int> GetPresentTodayCountAsync(string companyAdminId);
        Task<int> GetLateTodayCountAsync(string companyAdminId);
        Task<int> GetAbsentTodayCountAsync(string companyAdminId);

        Task<bool> AttendanceExistsTodayAsync(Guid employeeId);
        Task<List<Employees>> GetEmployeesMissingTimeInReminderAsync();
        Task<List<Attendances>> GetByEmployeeAndDateRangeAsync(Guid employeeId, DateTime startDate, DateTime endDate);

        Task<int> GetActiveTrackedAsync(string companyAdminId);

    }

}

