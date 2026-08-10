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
    public class PhilHealthContributionConfiguration
       : IEntityTypeConfiguration<PhilHealthContribution>
    {
        public void Configure(
            EntityTypeBuilder<PhilHealthContribution> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MinSalary)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MaxSalary)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ContributionRate)
                .HasColumnType("decimal(5,4)");

            builder.Property(x => x.EmployeeSharePercentage)
                .HasColumnType("decimal(5,4)");

            builder.Property(x => x.EmployerSharePercentage)
                .HasColumnType("decimal(5,4)");

            builder.HasData(
               new PhilHealthContribution
               {
                   Id = Guid.Parse("A2A34F4B-38B5-45B3-82B5-1C3E7D4A7F01"),

                   MinSalary = 10000m,

                   MaxSalary = 100000m,

                   ContributionRate = 0.05m,

                   EmployeeSharePercentage = 0.50m,

                   EmployerSharePercentage = 0.50m
               });
        }
    }
}
