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
    }
}
