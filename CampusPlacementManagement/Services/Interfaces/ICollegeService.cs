using CampusPlacementManagement.DTOs.College;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICollegeService
    {
        Task<CollegeResponseDto> RegisterCollegeAsync(
            CollegeRegisterDto request);

        Task<List<CollegeResponseDto>> GetAllCollegesAsync();

        Task<List<CollegeResponseDto>> GetPendingCollegesAsync();

        Task<CollegeResponseDto?> GetCollegeByIdAsync(int collegeId);

        Task<CollegeResponseDto?> UpdateCollegeAsync(
            int collegeId,
            CollegeUpdateDto request);

        Task<CollegeResponseDto?> ApproveCollegeAsync(int collegeId);

        Task<CollegeResponseDto?> RejectCollegeAsync(int collegeId);

        Task<CollegeResponseDto?> SuspendCollegeAsync(int collegeId);
    }
}