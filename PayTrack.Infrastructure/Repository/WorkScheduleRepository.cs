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
    public class WorkScheduleRepository: BaseRepository<WorkSchedules>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(AppDbContext context) : base(context) {}

        public async Task<List<WorkSchedules>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.WorkSchedules
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }
        public async Task DeleteRangeAsync(List<WorkSchedules> schedules)
        {
            _context.WorkSchedules.RemoveRange(schedules);
            await SaveChangesAsync();
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