using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class LeaveRequestConfiguration
        : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.EmployeeId)
                .IsRequired();


            builder.Property(x => x.LeaveType)
                .IsRequired()
                .HasConversion<int>();


            builder.Property(x => x.StartDate)
                .IsRequired()
                .HasColumnType("timestamp without time zone");


            builder.Property(x => x.EndDate)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

 
            builder.Property(x => x.TotalDays)
                .IsRequired();


            builder.Property(x => x.Reason)
                .HasMaxLength(1000)
                .IsRequired();


            builder.Property(x => x.AttachmentUrl)
                .HasMaxLength(500)
                .IsRequired(false);


            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(LeaveStatus.Pending);


            builder.Property(x => x.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);


            builder.Property(x => x.AdminRemarks)
                .HasMaxLength(1000)
                .IsRequired(false);


            builder.Property(x => x.ReviewedBy)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(x => x.CompanyAdminId)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(x => x.ReviewedAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CompanyAdmin)
                .WithMany()
                .HasForeignKey(x => x.CompanyAdminId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}