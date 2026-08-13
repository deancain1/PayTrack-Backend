using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class EmployeeSalary
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string CompanyAdminId { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal MonthlyRate { get; set; }
        public PayrollPolicy Policy { get; set; }
        public Employees Employee { get; set; } = null!;

    }
}
