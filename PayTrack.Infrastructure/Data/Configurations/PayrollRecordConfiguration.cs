using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class PayrollRecordConfiguration
        : IEntityTypeConfiguration<PayrollRecord>
    {
        public void Configure(EntityTypeBuilder<PayrollRecord> builder)
        {
            builder.ToTable("PayrollRecords");

          
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

        
            builder.Property(p => p.EmployeeId)
                .IsRequired();

          
            builder.Property(p => p.PayrollId)
                .IsRequired(false);

         
            builder.Property(p => p.ApplicationUserId)
                .IsRequired()
                .HasMaxLength(450);

         
            builder.Property(p => p.AttendanceId)
                .IsRequired();

          
            builder.Property(p => p.AmountEarned)
                .HasPrecision(18, 2)
                .IsRequired();

        
            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

         
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

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
                .OnDelete(DeleteBehavior.Cascade);

      
            builder.HasOne(p => p.Payroll)
                .WithMany()
                .HasForeignKey(p => p.PayrollId)
                .OnDelete(DeleteBehavior.SetNull);

   
            builder.HasIndex(p => p.EmployeeId);

            builder.HasIndex(p => p.AttendanceId);

            builder.HasIndex(p => p.ApplicationUserId);

            builder.HasIndex(p => p.PayrollId);
        }
    }
}