using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Enums
{
    public enum OvertimeStatus
    {
        PendingEmployeeResponse = 1,
        PendingAdminApproval = 2,
        Accepted = 3,
        Rejected = 4,
        Completed = 5,
        Cancelled = 6
    }
}
