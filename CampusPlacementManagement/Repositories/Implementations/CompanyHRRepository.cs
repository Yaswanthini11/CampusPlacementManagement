
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories.Implementations
{
    public class CompanyHRRepository : ICompanyHRRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyHRRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CompanyHR companyHR)
        {
            await _context.CompanyHRs.AddAsync(companyHR);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanyHR>> GetAllAsync()
        {
            return await _context.CompanyHRs
                .Include(hr => hr.User)
                .Include(hr => hr.Company)
                .Include(hr => hr.ApprovedByUser)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CompanyHR>> GetPendingAsync()
        {
            return await _context.CompanyHRs
                .Include(hr => hr.User)
                .Include(hr => hr.Company)
                .Where(hr => hr.User.AccountStatus == AccountStatus.Pending)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CompanyHR?> GetByIdAsync(int id)
        {
            return await _context.CompanyHRs
                .Include(hr => hr.User)
                .Include(hr => hr.Company)
                .Include(hr => hr.ApprovedByUser)
                .FirstOrDefaultAsync(hr => hr.CompanyHRId == id);
        }

        public async Task<CompanyHR?> GetByUserIdAsync(string userId)
        {
            return await _context.CompanyHRs
                .Include(hr => hr.User)
                .Include(hr => hr.Company)
                .Include(hr => hr.ApprovedByUser)
                .FirstOrDefaultAsync(hr => hr.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
