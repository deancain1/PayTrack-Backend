using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class Employees
    {
        public Guid Id { get; set; }
        public string EmployeeNumber { get; set; }
        public Guid DepartmentId { get; set; }
        public string Role { get; set; }
        public DateTime DateHired { get; set; }
        public string ApplicationUserId { get; set; }
        public string CompanyAdminId { get; set; }
        public Guid? WorkScheduleId { get; set; }
        public DateOnly? ScheduleEffectiveDate { get; set; }
        public string? QrCodeBase64 { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public EmployeeSalary Salary { get; set; }
        public WorkSchedules? WorkSchedule { get; set; }

        public bool UseCompanyLatePolicy { get; set; } = true;
        public Guid? AttendancePolicyId { get; set; }

        public AttendancePolicy? AttendancePolicy { get; set; }
        public Departments? Department { get; set; }
    }
}
