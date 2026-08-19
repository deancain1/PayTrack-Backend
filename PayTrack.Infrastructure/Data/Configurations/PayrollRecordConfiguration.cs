using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class PayrollRecordConfiguration
        : IEntityTypeConfiguration<PayrollRecord>
    {
        public void Configure(
            EntityTypeBuilder<PayrollRecord> builder)
        {
            // Employee
            builder.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);


            // Attendance
            builder.HasOne(p => p.Attendance)
                .WithMany()
                .HasForeignKey(p => p.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);


            // Application User
            builder.HasOne(p => p.ApplicationUser)
                .WithMany()
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Payroll
            builder.HasOne(p => p.Payroll)
                .WithMany()
                .HasForeignKey(p => p.PayrollId)
                .OnDelete(DeleteBehavior.SetNull);


            // Amount Earned
            builder.Property(p => p.AmountEarned)
                .HasPrecision(18, 2);
        }
    }
}