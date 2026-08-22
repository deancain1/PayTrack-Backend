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
    public class PayrollBatchConfiguration: IEntityTypeConfiguration<PayrollBatch>
    {
        public void Configure(EntityTypeBuilder<PayrollBatch> builder)
        {

            builder.ToTable("PayrollBatches");


            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();


            builder.Property(x => x.PeriodStart)
                .IsRequired();

            builder.Property(x => x.PeriodEnd)
                .IsRequired();


            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(
                    Domain.Enums.PayrollBatchStatus.Pending);

            builder.Property(x => x.ReleasedAt)
                .IsRequired(false);

            builder.Property(x => x.ReleasedBy)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(x => x.PaidAt)
                .IsRequired(false);

            builder.Property(x => x.PaymentMethod)
                .HasConversion<int?>()
                .IsRequired(false);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.PaidBy)
                .HasMaxLength(450)
                .IsRequired(false);


            builder.HasMany(x => x.Payrolls)
                .WithOne(x => x.PayrollBatch)
                .HasForeignKey(x => x.PayrollBatchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
