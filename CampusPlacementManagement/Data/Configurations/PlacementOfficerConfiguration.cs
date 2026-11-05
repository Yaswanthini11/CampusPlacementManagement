
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations
{
    public class PlacementOfficerConfiguration
        : IEntityTypeConfiguration<PlacementOfficer>
    {
        public void Configure(EntityTypeBuilder<PlacementOfficer> builder)
        {
            builder.HasKey(p => p.UserId);

            builder.Property(p => p.Designation)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Status)
                .HasConversion<int>();

            // One ApplicationUser has one PlacementOfficer profile.
            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PlacementOfficer>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One College can have multiple PlacementOfficers.
            builder.HasOne(p => p.College)
                .WithMany()
                .HasForeignKey(p => p.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Records which SuperAdmin approved the officer.
            builder.HasOne(p => p.ApprovedByUser)
                .WithMany()
                .HasForeignKey(p => p.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CollegeId);
        }
    }
}
