using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendances>
    {
        public void Configure(EntityTypeBuilder<Attendances> builder)
        {
           
            builder.HasKey(a => a.Id);

          
            builder.Property(a => a.EmployeeId)
                .IsRequired();

           
            builder.Property(a => a.Date)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

           
            builder.Property(a => a.TimeIn)
                .HasColumnType("timestamp without time zone");

          
            builder.Property(a => a.TimeOut)
                .HasColumnType("timestamp without time zone");

            
            builder.Property(a => a.HoursWorked)
                .IsRequired()
                .HasPrecision(10, 2);

           
            builder.Property(a => a.TimeOutReminderSentAt)
                .HasColumnType("timestamp without time zone");

            
            builder.Property(a => a.TimeInPhoto)
                .HasColumnType("text");

           
            builder.Property(a => a.TimeOutPhoto)
                .HasColumnType("text");

         
            builder.Property(a => a.IsLate)
                .IsRequired()
                .HasDefaultValue(false);

           
            builder.Property(a => a.LateMinutes)
                .IsRequired()
                .HasPrecision(10, 2);

            
            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(50);

           
            builder.HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}