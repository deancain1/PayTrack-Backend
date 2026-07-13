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
    }
}
