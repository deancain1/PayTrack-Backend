using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class PayrollRecord
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Guid? PayrollId { get; set; }
        public string ApplicationUserId { get; set; }

        public Guid AttendanceId { get; set; }

        public decimal AmountEarned { get; set; }

        public PayrollRecordStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Employees Employee { get; set; }
        public Attendances Attendance { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Payrolls? Payroll { get; set; }
    }
}
