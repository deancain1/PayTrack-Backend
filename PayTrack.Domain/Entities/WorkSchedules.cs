using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class WorkSchedules
    {
        public Guid Id { get; set; }

        public string ScheduleName { get; set; } = string.Empty;

        public ScheduleType ScheduleType { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int RequiredHours { get; set; }

        public int GracePeriodMinutes { get; set; }
        public TimeSpan AbsentCutoffTime { get; set; }

        public string CompanyAdminId { get; set; } = string.Empty;

        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Employees> Employees { get; set; } = new List<Employees>();

    }
}
