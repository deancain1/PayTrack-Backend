using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IEmployeesRepository
    {
        Task AddEmployeeAsync(Employees employee);
        IQueryable<Employees> GetEmployeesWithApplicationUser();
    }
}
