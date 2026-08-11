
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
    public class EmailOTPConfiguration : IEntityTypeConfiguration<EmailOTP>
    {
        public void Configure(EntityTypeBuilder<EmailOTP> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(x => x.Purpose)
                .HasConversion<int>();

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.EmailOtps)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.ApplicationUserId,
                x.Purpose,
                x.IsUsed
            });
        }
    }
}
