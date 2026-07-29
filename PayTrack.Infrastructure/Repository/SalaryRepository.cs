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
    public class SalaryRepository : BaseRepository<EmployeeSalary>, ISalaryRepository
    {
        public SalaryRepository(AppDbContext context) : base(context) { }

        public async Task<EmployeeSalary?> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmployeeSalaries
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }

        public async Task<EmployeeSalary?> GetByIdAndCompanyAdminAsync(Guid salaryId,string companyAdminId)
        {
            return await _context.EmployeeSalaries
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x =>
                    x.Id == salaryId &&
                    x.Employee.CompanyAdminId == companyAdminId);
        }
        public async Task UpdateAsync(EmployeeSalary salary)
        {
            _context.EmployeeSalaries.Update(salary);
            await _context.SaveChangesAsync();
        }
        public IQueryable<EmployeeSalary> GetEmployeeSalaryQueryByCompanyAdmin(string companyAdminId)
        {
            return _context.EmployeeSalaries
                .Include(s => s.Employee)
                .ThenInclude(e => e.ApplicationUser)
                .Where(e => e.CompanyAdminId == companyAdminId)
                .AsQueryable();
        }
        public async Task<List<EmployeeSalary>> GetAllAsync(string companyAdminId)
        {
            return await _context.EmployeeSalaries
                .Include(s => s.Employee)
                .Where(s => s.CompanyAdminId == companyAdminId)
                .ToListAsync();
        }

        public async Task<decimal> GetDailyRateAsync(Guid employeeId)
        {
            return await _context.EmployeeSalaries
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => x.Rate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmployeeSalary>> GetByIdsAsync(List<Guid> ids)
        {
          return await _context.EmployeeSalaries
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<EmployeeSalary> salaries)
        {
           _context.EmployeeSalaries.RemoveRange(salaries);
            await _context.SaveChangesAsync();
        }
    }
}
