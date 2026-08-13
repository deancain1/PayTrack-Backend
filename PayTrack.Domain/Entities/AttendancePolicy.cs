using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class AttendancePolicy
    {
        public Guid Id { get; set; }

        public string CompanyAdminId { get; set; } = string.Empty;
        public string PolicyName { get; set; } = string.Empty;
        public bool EnableLateDeduction { get; set; }
        public LateDeductionType LateDeductionType { get; set; }
        public decimal DeductionValue { get; set; }
        public bool IsDefault { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<Employees> Employees { get; set; }
            = new List<Employees>();
    }
}
