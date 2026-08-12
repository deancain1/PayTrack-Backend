using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Services
{
    public interface IPhilHealthService
    {
        Task<decimal> ComputeEmployeeContributionAsync(decimal monthlySalary, PayrollFrequency frequency);
            
    }
}
