using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class PhilHealthContribution
    {
        public Guid Id { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal ContributionRate { get; set; }
        public decimal EmployeeSharePercentage { get; set; }
        public decimal EmployerSharePercentage { get; set; }
    }
}
