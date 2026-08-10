using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class SSSContribution
    {
        public Guid Id { get; set; }

        public decimal MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public decimal RegularMonthlySalaryCredit { get; set; }

        public decimal MPFMonthlySalaryCredit { get; set; }

        public decimal TotalMonthlySalaryCredit { get; set; }

        public decimal EmployerRegularSSContribution { get; set; }

        public decimal EmployerMPFContribution { get; set; }

        public decimal EmployerECContribution { get; set; }

        public decimal EmployerTotalContribution { get; set; }

        public decimal EmployeeRegularSSContribution { get; set; }

        public decimal EmployeeMPFContribution { get; set; }

        public decimal EmployeeTotalContribution { get; set; }

        public decimal OverallTotalContribution { get; set; }
    }
}
