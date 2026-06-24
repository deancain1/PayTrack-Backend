using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayTrack.Domain.Entities;

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
        public DbSet<PayrollBatch> PayrollBatches { get; set; }
        public DbSet<WorkSchedules> WorkSchedules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employees → ApplicationUser (1:1)
            modelBuilder.Entity<Employees>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employees>(e => e.ApplicationUserId);

            // Departments → ApplicationUser (1:M)
            modelBuilder.Entity<Departments>()
                .HasOne(d => d.ApplicationUser)
                .WithMany(u => u.Departments)
                .HasForeignKey(d => d.ApplicationUserId);

            // PayrollRecord → Employee
            modelBuilder.Entity<PayrollRecord>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollRecord → Attendance
            modelBuilder.Entity<PayrollRecord>()
                .HasOne(p => p.Attendance)
                .WithMany()
                .HasForeignKey(p => p.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);


            // EmployeeSalary → Employee
            modelBuilder.Entity<EmployeeSalary>()
                .HasOne(es => es.Employee)
                 .WithOne(e => e.Salary)
                 .HasForeignKey<EmployeeSalary>(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> WorkSchedule
            modelBuilder.Entity<Employees>()
              .HasOne(e => e.WorkSchedule)
              .WithMany(ws => ws.Employees)
              .HasForeignKey(e => e.WorkScheduleId)
              .OnDelete(DeleteBehavior.SetNull);

            // WorkSchedule -> CompanyAdmin
            modelBuilder.Entity<WorkSchedules>()
             .HasOne(ws => ws.CompanyAdmin)
             .WithMany()
             .HasForeignKey(ws => ws.CompanyAdminId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}