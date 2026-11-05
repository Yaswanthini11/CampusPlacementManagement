using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
        }

        public async Task<Company?> GetByIdAsync(int companyId)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }

        public async Task<List<Company>> GetPendingAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.Status == CompanyStatus.Pending)
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Companies
                .AnyAsync(c => c.CompanyEmail == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}