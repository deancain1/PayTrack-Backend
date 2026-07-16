using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class SalaryAdvances
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
       
        public string ApplicationUserId { get; set; }

        public Employees Employee { get; set; }

        public decimal Amount { get; set; }

        public DateTime RequestDate { get; set; }

        public string Remarks { get; set; }

        public bool IsDeducted { get; set; }

        public Guid? PayrollId { get; set; }

        public Payrolls? Payroll { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
