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
    public class PayrollsConfiguration: IEntityTypeConfiguration<Payrolls>
    {
        public void Configure(EntityTypeBuilder<Payrolls> builder)
        {

            builder.ToTable("Payrolls");

            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.EmployeeId)
                .IsRequired();

            builder.Property(x => x.PayrollBatchId)
                .IsRequired();


            builder.Property(x => x.PeriodStart)
                .IsRequired();

            builder.Property(x => x.PeriodEnd)
                .IsRequired();


            builder.Property(x => x.MonthlyRateSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SalaryAdvanceDeduction)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SSSDeduction)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PhilHealthDeduction)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PagIbigDeduction)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.NetAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(
                    Domain.Enums.PayrollStatus.Pending);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ReleasedAt)
                .IsRequired(false);

            builder.Property(x => x.PaidAt)
                .IsRequired(false);

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PayrollBatch)
                .WithMany(x => x.Payrolls)
                .HasForeignKey(x => x.PayrollBatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PayrollBatchId);

            builder.HasIndex(x => x.EmployeeId);

            builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.PeriodStart,
                x.PeriodEnd
            });
        }
    }
}
