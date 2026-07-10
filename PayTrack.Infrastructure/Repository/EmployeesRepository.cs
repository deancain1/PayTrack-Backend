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
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly AppDbContext _context;
        public EmployeesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddEmployeeAsync(Employees employee)
        {
           await _context.Employees.AddAsync(employee);
           await _context.SaveChangesAsync();
        }

        public async Task<Employees?> GetEmployeeByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public IQueryable<Employees> GetEmployeesWithApplicationUser()
        {
            return _context.Employees
                .Include(e => e.ApplicationUser)
                .AsQueryable();
        }

        public async Task UpdateAsync(Employees employee)
        {
           _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Employees>> GetAllEmployeesByCompanyAdminAsync(string companyAdminId)
        {
            return await _context.Employees
                .Include(e => e.ApplicationUser)
                .Include(e => e.Department)
                .Where(e => e.CompanyAdminId == companyAdminId)
                .AsNoTracking()
                .ToListAsync();
        }
        public IQueryable<Employees> GetEmployeesByCompanyAdminQuery(string companyAdminId)
        {
            return _context.Employees
                .Include(e => e.ApplicationUser)
                .Include(e => e.Department)
                .Where(e => e.CompanyAdminId == companyAdminId)
                .AsQueryable();
        }
        public async Task<Employees?> GetByIdAndNumberAsync(Guid employeeId, string employeeNumber)
        {
            return await _context.Employees
                .Include(x => x.WorkSchedule)
                .Include(x => x.Salary)
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.EmployeeNumber == employeeNumber);
        }

        public async Task<Employees?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<List<Employees>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Employees
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.ApplicationUser)
                .Include(x => x.Salary)
                .ToListAsync();
        }

        public async Task<List<Employees>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.ApplicationUser)
                .ToListAsync();
        }
        public async Task<int> GetTotalEmployeesByCompanyAdminAsync(
           string companyAdminId)
        {
            return await _context.Employees
                .Where(e => e.CompanyAdminId == companyAdminId)
                .CountAsync();
        }
        public async Task<List<Employees>> GetEmployeesForAbsentAutomationAsync()
        {
            return await _context.Employees
                .Include(e => e.WorkSchedule)
                .Where(e => e.WorkSchedule != null)
                .ToListAsync();
        }

        public async Task<List<Employees>> GetEmployeesWithAssignedScheduleAsync()
        {
            return await _context.Employees
                .Include(e => e.WorkSchedule)
                .Include(e => e.ApplicationUser)
                .Where(e =>
                    e.WorkScheduleId != null &&
                    e.WorkSchedule != null &&
                    e.ScheduleEffectiveDate != null)
                .ToListAsync();
        }
    }
}

