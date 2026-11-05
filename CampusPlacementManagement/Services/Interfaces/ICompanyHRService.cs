
using CampusPlacementManagement.DTOs.CompanyHR;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICompanyHRService
    {
        Task<CompanyHRResponseDto> RegisterAsync(
            CompanyHRRegisterDto request);

        Task<List<CompanyHRResponseDto>> GetAllAsync();

        Task<List<CompanyHRResponseDto>> GetPendingAsync();

        Task<CompanyHRResponseDto?> ApproveAsync(
            int id, string approvedBy);

        Task<CompanyHRResponseDto?> RejectAsync(int id);

        Task<CompanyHRResponseDto?> GetMyProfileAsync(string userId);
    }
}

