using PayTrack.Application.DTOs;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IPayrollRepository
    {
     
        Task AddAsync(Payrolls payroll);
        Task<Payrolls?> GetByIdAsync(Guid id);
        Task UpdateAsync(Payrolls payroll);
        Task<bool> ExistsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd);
        Task<bool> HasGeneratedPayrollForDateAsync(Guid employeeId, DateTime requestDate);
        Task<List<Payrolls>> GetPayrollsByCompanyAndYearAsync(string companyAdminId, int year);
    }
}
