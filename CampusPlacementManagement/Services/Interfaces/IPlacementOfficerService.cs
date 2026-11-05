using CampusPlacementManagement.DTOs.PlacementOfficer;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface IPlacementOfficerService
    {
        Task<PlacementOfficerResponseDto> RegisterAsync(
            PlacementOfficerRegisterDto request);

        Task<List<PlacementOfficerResponseDto>> GetAllAsync();

        Task<List<PlacementOfficerResponseDto>> GetPendingAsync();

        Task<PlacementOfficerResponseDto?> GetByIdAsync(string userId);

        Task<PlacementOfficerResponseDto?> ApproveAsync(
            string userId,
            string approvedByUserId);

        Task<PlacementOfficerResponseDto?> RejectAsync(string userId);

        Task<PlacementOfficerResponseDto?> SuspendAsync(string userId);

        Task<PlacementOfficerResponseDto?> GetMyProfileAsync(string userId);
    }
}