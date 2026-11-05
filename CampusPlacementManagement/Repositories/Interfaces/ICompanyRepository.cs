using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task AddAsync(Company company);

        Task<Company?> GetByIdAsync(int companyId);

        Task<List<Company>> GetAllAsync();

        Task<List<Company>> GetPendingAsync();

        Task<bool> EmailExistsAsync(string email);

        Task SaveChangesAsync();
    }
}