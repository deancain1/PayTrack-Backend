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
    public class SalaryAdvanceConfiguration : IEntityTypeConfiguration<SalaryAdvances>
    {
        public void Configure(EntityTypeBuilder<SalaryAdvances> builder)
        {
            builder.HasOne(sa => sa.Employee)
                .WithMany()
                .HasForeignKey(sa => sa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.ApplicationUser)
                .WithMany()
                .HasForeignKey(sa => sa.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.Payroll)
                .WithMany()
                .HasForeignKey(sa => sa.PayrollId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(sa => sa.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
