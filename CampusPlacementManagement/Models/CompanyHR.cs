
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models
{
    public class CompanyHR
    {
        public int CompanyHRId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public string? Designation { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Relationships
        public ApplicationUser User { get; set; } = null!;

        public Company Company { get; set; } = null!;

        public ApplicationUser? ApprovedByUser { get; set; }
    }
}
