using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Persistence.Configurations
{
    public class PasswordResetOTPConfiguration
        : IEntityTypeConfiguration<PasswordResetOTP>
    {
        public void Configure(EntityTypeBuilder<PasswordResetOTP> builder)
        {
            builder.ToTable("PasswordResetOTPs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(256);

            builder.Property(x => x.OtpCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.IsUsed)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.Email);
        }
    }
}