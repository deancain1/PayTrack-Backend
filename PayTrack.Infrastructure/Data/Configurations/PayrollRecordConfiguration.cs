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
    public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
    {
        public void Configure(EntityTypeBuilder<PayrollRecord> builder)
        {
            builder.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Attendance)
                .WithMany()
                .HasForeignKey(p => p.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ApplicationUser)
                .WithMany()
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

           
        }
    }
}
