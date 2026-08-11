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
    public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedules>
    {
        public void Configure(EntityTypeBuilder<WorkSchedules> builder)
        {
            builder.HasOne(ws => ws.ApplicationUser)
                .WithMany()
                .HasForeignKey(ws => ws.CompanyAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
