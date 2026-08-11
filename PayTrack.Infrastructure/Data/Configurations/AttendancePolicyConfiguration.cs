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
    public class AttendancePolicyConfiguration : IEntityTypeConfiguration<AttendancePolicy>
    {
        public void Configure(EntityTypeBuilder<AttendancePolicy> builder)
        {
            builder.HasOne(ap => ap.ApplicationUser)
                .WithMany()
                .HasForeignKey(ap => ap.CompanyAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ap => ap.DeductionValue)
                .HasColumnType("decimal(18,2)");
        }
    }
}
