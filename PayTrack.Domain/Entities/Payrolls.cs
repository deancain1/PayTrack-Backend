using PayTrack.Domain.Enums;

namespace PayTrack.Domain.Entities
{
    public class Payrolls
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayrollBatchId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal MonthlyRateSnapshot { get; set; }
        public decimal RegularAmount { get; set; }
        public decimal OvertimeAmount { get; set; }
        public decimal PaidLeaveAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SalaryAdvanceDeduction { get; set; }
        public decimal SSSDeduction { get; set; }
        public decimal PhilHealthDeduction { get; set; }
        public decimal PagIbigDeduction { get; set; }
        public decimal NetAmount { get; set; }
        public PayrollStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public Employees Employee { get; set; } = null!;
        public PayrollBatch PayrollBatch { get; set; } = null!;
    }
}