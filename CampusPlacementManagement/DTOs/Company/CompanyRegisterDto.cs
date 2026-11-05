using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Company
{
    public class CompanyRegisterDto
    {
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string CompanyEmail { get; set; } = string.Empty;

        [Url]
        [StringLength(255)]
        public string? Website { get; set; }

        [Required]
        [StringLength(100)]
        public string Industry { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}