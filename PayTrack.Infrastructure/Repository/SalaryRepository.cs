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
    public class SalaryRepository : ISalaryRepository
    {
        private readonly AppDbContext _context;
        public SalaryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EmployeeSalary?> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmployeeSalaries
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }
        public async Task AddAsync(EmployeeSalary salary)
        {
            await _context.EmployeeSalaries.AddAsync(salary);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmployeeSalary salary)
        {
            _context.EmployeeSalaries.Update(salary);
            await _context.SaveChangesAsync();
        }
        public IQueryable<EmployeeSalary> GetEmployeeSalaryQuery()
        {
            return _context.EmployeeSalaries
                .Include(e => e.ApplicationUser)
                .AsNoTracking();
        }

    }
}
