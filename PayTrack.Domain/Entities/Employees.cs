using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class Employees
    {
        public Guid Id { get; set; }
        public string EmployeeNumber { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public DateTime DateHired { get; set; }
        public string ApplicationUserId { get; set; }
        public string? QrCodeBase64 { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public EmployeeSalary Salary { get; set; }
    }
}
