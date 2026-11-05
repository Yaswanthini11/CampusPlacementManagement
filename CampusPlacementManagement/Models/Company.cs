using CampusPlacementManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string CompanyEmail { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Website { get; set; }

        [Required]
        [StringLength(100)]
        public string Industry { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        public string? Description { get; set; }

        public CompanyStatus Status { get; set; }
            = CompanyStatus.Pending;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}