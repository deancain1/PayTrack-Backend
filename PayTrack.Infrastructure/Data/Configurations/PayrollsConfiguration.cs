using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class PayrollsConfiguration : IEntityTypeConfiguration<Payrolls>
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
                .IsRequired()
                .HasColumnType("timestamp without time zone");

           
            builder.Property(x => x.PeriodEnd)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

           
            builder.Property(x => x.MonthlyRateSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            
            builder.Property(x => x.RegularAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.OvertimeAmount)
                .HasPrecision(18, 2)
                .IsRequired();

          
            builder.Property(x => x.PaidLeaveAmount)
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
                .HasDefaultValue(PayrollStatus.Pending);

          
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

         
            builder.Property(x => x.ReleasedAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");

          
            builder.Property(x => x.PaidAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");

          
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