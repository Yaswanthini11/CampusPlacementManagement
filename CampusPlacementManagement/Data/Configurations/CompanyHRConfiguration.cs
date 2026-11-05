
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations
{
    public class CompanyHRConfiguration
        : IEntityTypeConfiguration<CompanyHR>
    {
        public void Configure(EntityTypeBuilder<CompanyHR> builder)
        {
            builder.HasKey(hr => hr.CompanyHRId);

            builder.Property(hr => hr.UserId)
                .IsRequired();

            builder.Property(hr => hr.Designation)
                .HasMaxLength(100);

            // One ApplicationUser has one CompanyHR profile.
            builder.HasOne(hr => hr.User)
                .WithOne()
                .HasForeignKey<CompanyHR>(hr => hr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Company can have multiple HR representatives.
            builder.HasOne(hr => hr.Company)
                .WithMany()
                .HasForeignKey(hr => hr.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // The approving SuperAdmin is an ApplicationUser.
            builder.HasOne(hr => hr.ApprovedByUser)
                .WithMany()
                .HasForeignKey(hr => hr.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(hr => hr.CompanyId);

            builder.HasIndex(hr => hr.UserId)
                .IsUnique();
        }
    }
}
