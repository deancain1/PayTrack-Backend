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

        public IQueryable<Employees> GetEmployeesQuery()
        {
            return _context.Employees
                .Include(e => e.ApplicationUser)
                .AsNoTracking();
        }
    }
}
