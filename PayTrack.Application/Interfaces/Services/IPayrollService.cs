using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Services
{
    public interface IPayrollService
    {
        decimal Calculate(EmployeeSalary salary, Attendances attendance, WorkSchedules schedule);
        Task<decimal> CalculateCurrentEarningsAsync(Guid employeeId, DateTime startDate, DateTime endDate);
    }
}
