
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Students
{
    public class StudentUpdateDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(100)]
        public string? Degree { get; set; }

        [Range(2020, 2100)]
        public int? GraduationYear { get; set; }

        [Range(typeof(decimal), "0", "10")]
        public decimal? CGPA { get; set; }

        [Range(0, 100)]
        public int? ActiveBacklogs { get; set; }

        [MaxLength(500)]
        public string? ResumeUrl { get; set; }
    }
}
