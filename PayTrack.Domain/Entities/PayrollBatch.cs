using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class PayrollBatch
    {
        public Guid Id { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public PayrollBatchStatus Status { get; set; } = PayrollBatchStatus.Pending;
        public DateTime? ReleasedAt { get; set; }
        public string? ReleasedBy { get; set; }

        public DateTime? PaidAt { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? PaidBy { get; set; }

        public ICollection<Payrolls> Payrolls { get; set; } = [];
    }
}
