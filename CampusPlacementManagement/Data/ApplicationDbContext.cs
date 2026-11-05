
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<College> Colleges { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<College>(entity =>
            {
                entity.HasKey(c => c.CollegeId);

                entity.HasIndex(c => c.CollegeCode)
                    .IsUnique();

                entity.Property(c => c.CollegeName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.CollegeCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.OfficialEmail)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(c => c.Status)
                    .HasConversion<int>();
            });
        }
    }
}
