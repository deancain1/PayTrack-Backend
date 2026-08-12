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
    internal class EmployeeNotificationPreferenceConfiguration : IEntityTypeConfiguration<EmployeeNotificationPreference>
    {
        public void Configure(
          EntityTypeBuilder<EmployeeNotificationPreference> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            builder.HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            builder.Property(x => x.SalaryAdvanceApproved)
                .HasDefaultValue(true);

            builder.Property(x => x.PayrollGenerated)
                .HasDefaultValue(true);

            builder.Property(x => x.PayrollReleased)
                .HasDefaultValue(true);
        }
    }
}
