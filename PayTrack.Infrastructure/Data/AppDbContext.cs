using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Employees> Employees => Set<Employees>();
        public DbSet<Departments> Departments => Set<Departments>();
        public DbSet<Attendances> Attendances => Set<Attendances>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employees>()
            .HasOne(e => e.ApplicationUser)
            .WithOne(u => u.Employee)
             .HasForeignKey<Employees>(e => e.ApplicationUserId);

            modelBuilder.Entity<Departments>()
            .HasOne(d => d.ApplicationUser)
            .WithMany(u => u.Departments)
            .HasForeignKey(d => d.ApplicationUserId);

            base.OnModelCreating(modelBuilder);

        }
    }
}
