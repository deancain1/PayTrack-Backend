using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    public class WorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly AppDbContext _context;
        public WorkScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(WorkSchedules schedule)
        {
            await _context.WorkSchedules.AddAsync(schedule);

            await _context.SaveChangesAsync();

            return schedule.Id;
        }

        public async Task UpdateAsync(WorkSchedules schedule)
        {
            _context.WorkSchedules.Update(schedule);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var schedule =
                await GetByIdAsync(id);

            if (schedule == null)
                return;

            _context.WorkSchedules.Remove(schedule);

            await _context.SaveChangesAsync();
        }

        public async Task<WorkSchedules?> GetByIdAsync(Guid id)
        {
            return await _context.WorkSchedules
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //public async Task<List<WorkSchedules>>GetAllByAdminAsync(
        //        string companyAdminId)
        //{
        //    return await _context.WorkSchedules
        //        .Where(x =>
        //            x.CompanyAdminId ==
        //            companyAdminId)
        //        .ToListAsync();
        //}

        public async Task AssignScheduleToEmployeeAsync(
            Guid employeeId,
            Guid scheduleId, DateTime scheduleEffectiveDate)
        {
            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(x =>
                        x.Id == employeeId);

            if (employee == null)
                throw new Exception(
                    "Employee not found");

            employee.WorkScheduleId =
                scheduleId;

            employee.ScheduleEffectiveDate = scheduleEffectiveDate;

            await _context.SaveChangesAsync();
        }
        public IQueryable<WorkSchedules> GetSchedulesByCompanyAdminQuery(string companyAdminId)
        {
            return _context.WorkSchedules
            .Where(x => x.CompanyAdminId == companyAdminId)
            .AsNoTracking();
        }

        public IQueryable<WorkSchedules> GetWorkSchedulesQuery(string companyAdminId)
        {
          return _context.WorkSchedules
            .Where(x => x.CompanyAdminId == companyAdminId)
            .AsNoTracking();
        }
    }
}