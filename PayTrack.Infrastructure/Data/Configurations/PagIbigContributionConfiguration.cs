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
    public class PagIbigContributionConfiguration
       : IEntityTypeConfiguration<PagIbigContribution>
    {
        public void Configure(
            EntityTypeBuilder<PagIbigContribution> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SalaryThreshold)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.LowerEmployeeRate)
                .HasColumnType("decimal(5,4)");

            builder.Property(x => x.HigherEmployeeRate)
                .HasColumnType("decimal(5,4)");

            builder.Property(x => x.EmployerRate)
                .HasColumnType("decimal(5,4)");

            builder.Property(x => x.MaximumCompensation)
                .HasColumnType("decimal(18,2)");

            builder.HasData(
               new PagIbigContribution
               {
                   Id = Guid.Parse("D42D69AF-16F6-4D0B-B45F-0B95D5AAB001"),

                   // Salary up to this amount uses the lower employee rate
                   SalaryThreshold = 1500m,

                   // 1%
                   LowerEmployeeRate = 0.01m,

                   // 2%
                   HigherEmployeeRate = 0.02m,

                   // Employer share
                   EmployerRate = 0.02m,

                   // Maximum compensation used for computation
                   MaximumCompensation = 5000m
               });

        }
    }
}
