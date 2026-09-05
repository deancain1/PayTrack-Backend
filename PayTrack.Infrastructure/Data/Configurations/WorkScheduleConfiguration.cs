using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayTrack.Domain.Entities;

namespace PayTrack.Infrastructure.Data.Configurations
{
    public class WorkScheduleConfiguration: IEntityTypeConfiguration<WorkSchedules>
    {
        public void Configure(EntityTypeBuilder<WorkSchedules> builder)
        {
            builder.ToTable("WorkSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ScheduleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ScheduleType)
                .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnType("time without time zone");

            builder.Property(x => x.EndTime)
                .HasColumnType("time without time zone");

            builder.Property(x => x.EarliestStartTime)
                .HasColumnType("time without time zone");

            builder.Property(x => x.LatestStartTime)
                .HasColumnType("time without time zone");

            builder.Property(x => x.RequiredHours)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.GracePeriodMinutes)
                .IsRequired()
                .HasDefaultValue(15);

            builder.Property(x => x.AbsentCutoffTime)
                .HasColumnType("time without time zone");

            builder.Property(x => x.WorkingDays)
                .IsRequired();

            builder.HasOne(ws => ws.ApplicationUser)
               .WithMany()
               .HasForeignKey(ws => ws.CompanyAdminId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ScheduleName);
        }
    }
}