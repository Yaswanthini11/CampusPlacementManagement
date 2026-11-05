using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories.Implementations
{
    public class PlacementOfficerRepository
        : IPlacementOfficerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacementOfficerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PlacementOfficer officer)
        {
            await _context.PlacementOfficers.AddAsync(officer);
        }

        public async Task<PlacementOfficer?> GetByIdAsync(string userId)
        {
            return await _context.PlacementOfficers
                .Include(p => p.User)
                .Include(p => p.College)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<PlacementOfficer>> GetAllAsync()
        {
            return await _context.PlacementOfficers
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.College)
                .OrderBy(p => p.User.FullName)
                .ToListAsync();
        }

        public async Task<List<PlacementOfficer>> GetPendingAsync()
        {
            return await _context.PlacementOfficers
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.College)
                .Where(p => p.Status == PlacementOfficerStatus.Pending)
                .OrderBy(p => p.User.FullName)
                .ToListAsync();
        }

        public async Task<bool> CollegeExistsAndApprovedAsync(int collegeId)
        {
            return await _context.Colleges.AnyAsync(c =>
                c.CollegeId == collegeId &&
                c.Status == CollegeStatus.Approved);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}