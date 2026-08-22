using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class OvertimeRequest
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Guid? PayrollId { get; set; }
        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public decimal RequestedHours { get; set; }

        public decimal ActualHours { get; set; }

        public decimal OvertimeRate { get; set; }

        public decimal OvertimePay { get; set; }

        public string? Reason { get; set; }

        public string? EmployeeRemarks { get; set; }

        public string? AdminRemarks { get; set; }

        public OvertimeSource Source { get; set; }

        public OvertimeEmployeeResponse EmployeeResponse { get; set; }

        public OvertimeStatus Status { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? RespondedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Guid? PayrollRecordId { get; set; }

        // Navigation properties
        public Employees Employee { get; set; } = null!;

        public PayrollRecord? PayrollRecord { get; set; }
        public Payrolls? Payroll { get; set; }
    }
}
