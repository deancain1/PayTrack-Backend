using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IAttendancePolicyService
    {
        string DetermineStatus(WorkSchedules schedule, DateTime timeIn);

        DateTime CalculateExpectedTimeOut(WorkSchedules schedule, DateTime timeIn);
    }
}
