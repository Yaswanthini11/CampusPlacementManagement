
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Students
{
    public class BulkStudentEnrollDto
    {
        [Required]
        [MinLength(1)]
        public List<StudentEnrollDto> Students { get; set; } = new();
    }
}
