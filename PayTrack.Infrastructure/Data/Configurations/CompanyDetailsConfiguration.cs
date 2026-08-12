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
    public class CompanyDetailsConfiguration : IEntityTypeConfiguration<CompanyDetails>
    {
        public void Configure(EntityTypeBuilder<CompanyDetails> builder)
        {
            builder.ToTable("CompanyDetails");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BusinessType)
                .HasMaxLength(100);

            builder.HasOne(x => x.ApplicatonUser)
                .WithOne(x => x.CompanyDetails)
                .HasForeignKey<CompanyDetails>(x => x.CompanyAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
