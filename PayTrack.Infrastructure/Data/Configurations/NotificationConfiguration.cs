
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
    public class NotificationConfiguration : IEntityTypeConfiguration<CompanyAdminNotification>
    {
        public void Configure(EntityTypeBuilder<CompanyAdminNotification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Type)
                .HasConversion<int>();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
