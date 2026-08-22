using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class OvertimeRequestConfiguration
        : IEntityTypeConfiguration<OvertimeRequest>
    {
        public void Configure(
            EntityTypeBuilder<OvertimeRequest> builder)
        {
            builder.ToTable("OvertimeRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestedHours)
                .HasPrecision(18, 2);

            builder.Property(x => x.ActualHours)
                .HasPrecision(18, 2);

            builder.Property(x => x.OvertimeRate)
                .HasPrecision(18, 2);

            builder.Property(x => x.OvertimePay)
                .HasPrecision(18, 2);

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.Property(x => x.EmployeeRemarks)
                .HasMaxLength(500);

            builder.Property(x => x.AdminRemarks)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(x => x.PayrollRecord)
                .WithMany()
                .HasForeignKey(x => x.PayrollRecordId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(x => x.Payroll)
                .WithMany()
                .HasForeignKey(x => x.PayrollId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.Date
            });
        }
    }
}