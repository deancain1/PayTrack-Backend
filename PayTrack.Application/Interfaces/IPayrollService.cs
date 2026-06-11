using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IPayrollService
    {
        decimal Calculate(
            EmployeeSalary salary,
            decimal hoursWorked);
    }
}
