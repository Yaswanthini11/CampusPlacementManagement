
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

        public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
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


            // Placement Officer configuration
            builder.Entity<PlacementOfficer>(entity =>
            {
                entity.HasKey(p => p.UserId);

                entity.Property(p => p.Designation)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Status)
                    .HasConversion<int>();

                // One ApplicationUser has one PlacementOfficer profile
                entity.HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<PlacementOfficer>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One College can have multiple Placement Officers
                entity.HasOne(p => p.College)
                    .WithMany()
                    .HasForeignKey(p => p.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // The approving SuperAdmin is an ApplicationUser
                entity.HasOne(p => p.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.CollegeId);
            });
        }
    }
}
