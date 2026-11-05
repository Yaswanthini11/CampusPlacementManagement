using CampusPlacementManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models
{
    public class PlacementOfficer
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public int CollegeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        public PlacementOfficerStatus Status { get; set; }
            = PlacementOfficerStatus.Pending;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;

        public College College { get; set; } = null!;

        public ApplicationUser? ApprovedByUser { get; set; }
    }
}