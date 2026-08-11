using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class EmployeeSalaryConfiguration : IEntityTypeConfiguration<EmployeeSalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalary> builder)
        {
            builder.HasOne(es => es.Employee)
                .WithOne(e => e.Salary)
                .HasForeignKey<EmployeeSalary>(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(es => es.Rate)
                .HasColumnType("decimal(18,2)");
        }
    }
}
