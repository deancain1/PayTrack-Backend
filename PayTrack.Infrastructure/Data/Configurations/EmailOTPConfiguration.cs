using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class EmailOTPConfiguration : IEntityTypeConfiguration<EmailOTP>
    {
        public void Configure(EntityTypeBuilder<EmailOTP> builder)
        {

            builder.HasKey(x => x.Id);


            builder.Property(x => x.ApplicationUserId)
                .IsRequired();


            builder.Property(x => x.Code)
                .HasMaxLength(6)
                .IsRequired();


            builder.Property(x => x.Purpose)
                .HasConversion<int>()
                .IsRequired();


            builder.Property(x => x.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");


            builder.Property(x => x.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

 
            builder.Property(x => x.FailedAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.EmailOtps)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(x => new
            {
                x.ApplicationUserId,
                x.Purpose,
                x.IsUsed
            });
        }
    }
}