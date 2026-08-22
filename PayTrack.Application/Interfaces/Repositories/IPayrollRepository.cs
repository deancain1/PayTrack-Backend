using PayTrack.Application.DTOs;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IPayrollRepository : IBaseRepository<Payrolls>
    {
    
        Task<bool> ExistsAsync(Guid employeeId, DateTime periodStart, DateTime periodEnd);
        Task<bool> HasGeneratedPayrollForDateAsync(Guid employeeId, DateTime requestDate);
        Task<List<Payrolls>> GetPayrollsByCompanyAndYearAsync(string companyAdminId, int year);
        Task<List<Payrolls>> GetEmployeePayrollsAsync(Guid employeeId);
    }
}
