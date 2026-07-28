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
    public class DepartmentRepository: BaseRepository<Departments>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context) : base(context) { }


        public IQueryable<Departments> GetDepartmentsQuery()
        {
            return _context.Departments
                 .AsNoTracking();
                
        }
        public async Task<List<Departments>> GetDepartmentsByUserIdAsync(string userId)
        {
            return await _context.Departments
                .Where(d => d.ApplicationUserId == userId)
                .ToListAsync();
        }

        public async Task<List<Departments>> GetByIdsAsync(List<Guid> ids)
        {
          return await _context.Departments
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<Departments> departments)
        {
            _context.Departments.RemoveRange(departments);
            await SaveChangesAsync();
        }
    }
}
