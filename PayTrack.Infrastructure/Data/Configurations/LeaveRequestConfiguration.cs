using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class LeaveRequestConfiguration
        : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LeaveType)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.TotalDays)
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.AttachmentUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.IsPaid)
              .IsRequired()
              .HasDefaultValue(false);

            builder.Property(x => x.AdminRemarks)
                .HasMaxLength(1000);

            builder.Property(x => x.CompanyAdminId)
                .HasMaxLength(450);

            builder.Property(x => x.ReviewedBy)
                .HasMaxLength(256);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Employee relationship
            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Company Admin relationship
            builder.HasOne(x => x.CompanyAdmin)
                .WithMany()
                .HasForeignKey(x => x.CompanyAdminId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
