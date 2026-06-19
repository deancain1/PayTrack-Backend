using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class Payrolls
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayrollBatchId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public PayrollStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public Employees Employee { get; set; } = null!;
        public PayrollBatch PayrollBatch { get; set; }
    }
}
