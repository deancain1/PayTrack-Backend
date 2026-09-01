using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class SalaryAdvanceConfiguration
        : IEntityTypeConfiguration<SalaryAdvances>
    {
        public void Configure(EntityTypeBuilder<SalaryAdvances> builder)
        {
            builder.ToTable("SalaryAdvances");

           
            builder.HasKey(sa => sa.Id);

        
            builder.Property(sa => sa.EmployeeId)
                .IsRequired();

          
            builder.Property(sa => sa.ApplicationUserId)
                .IsRequired()
                .HasMaxLength(450);

            
            builder.Property(sa => sa.Remarks)
                .IsRequired()
                .HasMaxLength(1000);

      
            builder.Property(sa => sa.Amount)
                .HasPrecision(18, 2)
                .IsRequired();


            builder.Property(sa => sa.RequestDate)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            
            builder.Property(sa => sa.IsDeducted)
                .IsRequired()
                .HasDefaultValue(false);

            
            builder.Property(sa => sa.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(SalaryAdvanceStatus.Pending);

         
            builder.Property(sa => sa.PayrollId)
                .IsRequired(false);

            // Employee relationship
            builder.HasOne(sa => sa.Employee)
                .WithMany()
                .HasForeignKey(sa => sa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser relationship
            builder.HasOne(sa => sa.ApplicationUser)
                .WithMany()
                .HasForeignKey(sa => sa.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payroll relationship
            builder.HasOne(sa => sa.Payroll)
                .WithMany()
                .HasForeignKey(sa => sa.PayrollId)
                .OnDelete(DeleteBehavior.SetNull);

           
            builder.HasIndex(sa => sa.EmployeeId);

            builder.HasIndex(sa => sa.ApplicationUserId);

            builder.HasIndex(sa => sa.PayrollId);
        }
    }
}