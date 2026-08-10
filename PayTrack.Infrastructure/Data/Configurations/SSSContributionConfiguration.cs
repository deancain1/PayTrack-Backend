using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class SSSContributionConfiguration
        : IEntityTypeConfiguration<SSSContribution>
    {
        public void Configure(
            EntityTypeBuilder<SSSContribution> builder)
        {
            builder.HasKey(x => x.Id);

            // Compensation Range
            builder.Property(x => x.MinSalary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.MaxSalary)
                .HasColumnType("decimal(18,2)");

            // Monthly Salary Credit (MSC)
            builder.Property(x => x.RegularMonthlySalaryCredit)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.MPFMonthlySalaryCredit)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TotalMonthlySalaryCredit)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Employer
            builder.Property(x => x.EmployerRegularSSContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EmployerMPFContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EmployerECContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EmployerTotalContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Employee
            builder.Property(x => x.EmployeeRegularSSContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EmployeeMPFContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EmployeeTotalContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Overall Total
            builder.Property(x => x.OverallTotalContribution)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}