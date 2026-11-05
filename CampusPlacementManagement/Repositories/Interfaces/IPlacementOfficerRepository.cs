using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface IPlacementOfficerRepository
    {
        Task AddAsync(PlacementOfficer officer);

        Task<PlacementOfficer?> GetByIdAsync(string userId);

        Task<List<PlacementOfficer>> GetAllAsync();

        Task<List<PlacementOfficer>> GetPendingAsync();

        Task<bool> CollegeExistsAndApprovedAsync(int collegeId);

        Task SaveChangesAsync();
    }
}