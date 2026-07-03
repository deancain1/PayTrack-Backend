using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class Attendances
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public decimal HoursWorked { get; set; }
        public string? TimeInPhoto { get; set; }
        public string? TimeOutPhoto { get; set; }
        public bool IsLate { get; set; }
        public decimal LateMinutes { get; set; }
        public string Status { get; set; }
        public Employees Employee { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
