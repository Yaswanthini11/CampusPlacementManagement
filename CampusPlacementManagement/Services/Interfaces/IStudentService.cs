
using CampusPlacementManagement.DTOs.Students;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponseDto> EnrollAsync(
            StudentEnrollDto request,
            string placementOfficerUserId);

        Task<List<StudentResponseDto>> BulkEnrollAsync(
            BulkStudentEnrollDto request,
            string placementOfficerUserId);

        Task<List<StudentResponseDto>> GetAllAsync(
            string userId,
            string role);

        Task<List<StudentResponseDto>> GetPendingAsync(
            string placementOfficerUserId);

        Task<StudentResponseDto?> GetByIdAsync(
            int studentId,
            string userId,
            string role);

        Task<StudentResponseDto?> ApproveAsync(
            int studentId,
            string placementOfficerUserId);

        Task<StudentResponseDto?> RejectAsync(
            int studentId,
            string placementOfficerUserId);

        Task<StudentResponseDto?> SuspendAsync(
            int studentId,
            string userId,
            string role);

        Task<StudentResponseDto?> UpdateAsync(
            int studentId,
            StudentUpdateDto request,
            string userId,
            string role);

        Task<StudentResponseDto?> GetMyProfileAsync(string userId);
    }
}
