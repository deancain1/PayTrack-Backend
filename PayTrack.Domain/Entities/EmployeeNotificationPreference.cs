using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class EmployeeNotificationPreference
    {
        public Guid Id { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        public bool SalaryAdvanceApproved { get; set; } = true;

        public bool SalaryAdvanceRejected { get; set; } = true;

        public bool PayrollGenerated { get; set; } = true;

        public bool PayrollReleased { get; set; } = true;
    }
}
