using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employees>
    {
        public void Configure(EntityTypeBuilder<Employees> builder)
        {
            builder.HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employees>(e => e.ApplicationUserId);

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.WorkSchedule)
                .WithMany(ws => ws.Employees)
                .HasForeignKey(e => e.WorkScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.AttendancePolicy)
                .WithMany(ap => ap.Employees)
                .HasForeignKey(e => e.AttendancePolicyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(e => e.UseCompanyLatePolicy)
                .HasDefaultValue(true);
        }
    }
}
