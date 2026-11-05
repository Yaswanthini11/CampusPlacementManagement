using CampusPlacementManagement.DTOs.Company;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto> RegisterAsync(
            CompanyRegisterDto request);

        Task<List<CompanyResponseDto>> GetAllAsync();

        Task<List<CompanyResponseDto>> GetPendingAsync();

        Task<CompanyResponseDto?> GetByIdAsync(int companyId);

        Task<CompanyResponseDto?> UpdateAsync(
            int companyId,
            CompanyUpdateDto request);

        Task<CompanyResponseDto?> ApproveAsync(int companyId);

        Task<CompanyResponseDto?> RejectAsync(int companyId);

        Task<CompanyResponseDto?> SuspendAsync(int companyId);
    }
}