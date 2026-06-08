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

        public DateTime ScanTime { get; set; }

        public string? PhotoPath { get; set; }

        public Employees Employee { get; set; } = null!;
    }
}
