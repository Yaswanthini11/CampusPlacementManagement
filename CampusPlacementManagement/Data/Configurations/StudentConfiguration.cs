
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations
{
    public class StudentConfiguration
        : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.StudentId);

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.RegisterNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Degree)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.CGPA)
                .HasPrecision(4, 2)
                .IsRequired();

            builder.Property(s => s.ActiveBacklogs)
                .IsRequired();

            builder.Property(s => s.ResumeUrl)
                .HasMaxLength(500);

            builder.Property(s => s.Status)
                .HasConversion<int>()
                .IsRequired();

            // One Identity user has one Student profile.
            builder.HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One college has many students.
            builder.HasOne(s => s.College)
                .WithMany()
                .HasForeignKey(s => s.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Placement Officer can enroll many students.
            builder.HasOne(s => s.PlacementOfficer)
                .WithMany()
                .HasForeignKey(s => s.PlacementOfficerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Register number must be unique within a college.
            builder.HasIndex(s => new
            {
                s.CollegeId,
                s.RegisterNumber
            }).IsUnique();

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.HasIndex(s => s.PlacementOfficerUserId);
        }
    }
}
