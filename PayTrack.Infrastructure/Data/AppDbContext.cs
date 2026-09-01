using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data.Configurations;

namespace PayTrack.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employees> Employees => Set<Employees>();
        public DbSet<Departments> Departments => Set<Departments>();
        public DbSet<Attendances> Attendances => Set<Attendances>();
        public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
        public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
        public DbSet<Payrolls> Payrolls => Set<Payrolls>();
        public DbSet<PayrollBatch> PayrollBatches => Set<PayrollBatch>();
        public DbSet<WorkSchedules> WorkSchedules => Set<WorkSchedules>();
        public DbSet<AttendancePolicy> AttendancePolicies => Set<AttendancePolicy>();
        public DbSet<SalaryAdvances> SalaryAdvances => Set<SalaryAdvances>();
        public DbSet<EmailOTP> EmailOTPs => Set<EmailOTP>();
        public DbSet<CompanyPayrollSetting> CompanyPayrollSettings => Set<CompanyPayrollSetting>();
        public DbSet<EmployeeNotificationPreference> EmployeeNotificationPreferences => Set<EmployeeNotificationPreference>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<OvertimeRequest> OvertimeRequests => Set<OvertimeRequest>();

        //Company Details
        public DbSet<CompanyDetails> CompanyDetails => Set<CompanyDetails>();
        //notifications
        public DbSet<CompanyAdminNotification> Notifications => Set<CompanyAdminNotification>();
        public DbSet<EmployeeNotification> EmployeeNotifications => Set<EmployeeNotification>();

        //taxes
        public DbSet<SSSContribution> SSSContributions => Set<SSSContribution>();
        public DbSet<PhilHealthContribution> PhilHealthContributions =>Set<PhilHealthContribution>();
        public DbSet<PagIbigContribution> PagIbigContributions => Set<PagIbigContribution>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly
            );
        }
    }
}