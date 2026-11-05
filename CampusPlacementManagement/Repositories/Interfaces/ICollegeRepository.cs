using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICollegeRepository
    {
        Task AddAsync(College college);

        Task<College?> GetByIdAsync(int collegeId);

        Task<List<College>> GetAllAsync();

        Task<List<College>> GetPendingCollegesAsync();

        Task<bool> CollegeCodeExistsAsync(
            string collegeCode,
            int? excludeCollegeId = null);

        Task SaveChangesAsync();
    }
}