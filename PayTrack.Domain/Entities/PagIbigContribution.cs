using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class PagIbigContribution
    {
        public Guid Id { get; set; }

        // Salary threshold where the employee rate changes
        public decimal SalaryThreshold { get; set; }

        // Example: 0.01 = 1%
        public decimal LowerEmployeeRate { get; set; }

        // Example: 0.02 = 2%
        public decimal HigherEmployeeRate { get; set; }

        public decimal EmployerRate { get; set; }

        // Maximum monthly salary used for computation
        public decimal MaximumCompensation { get; set; }
    }
}
