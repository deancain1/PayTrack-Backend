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

        public string EmployeeNumber { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateTime DateHired { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
