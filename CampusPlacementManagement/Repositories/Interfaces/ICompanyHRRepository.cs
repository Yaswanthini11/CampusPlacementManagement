
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICompanyHRRepository
    {
        Task AddAsync(CompanyHR companyHR);

        Task<List<CompanyHR>> GetAllAsync();

        Task<List<CompanyHR>> GetPendingAsync();

        Task<CompanyHR?> GetByIdAsync(int id);

        Task<CompanyHR?> GetByUserIdAsync(string userId);

        Task SaveChangesAsync();
    }
}
