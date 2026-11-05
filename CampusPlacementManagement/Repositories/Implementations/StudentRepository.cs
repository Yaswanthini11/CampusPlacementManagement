
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.College)
                .Include(s => s.PlacementOfficer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Student>> GetByCollegeIdAsync(int collegeId)
        {
            return await _context.Students
                .Where(s => s.CollegeId == collegeId)
                .Include(s => s.User)
                .Include(s => s.College)
                .Include(s => s.PlacementOfficer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Student>> GetPendingByCollegeIdAsync(
            int collegeId)
        {
            return await _context.Students
                .Where(s => s.CollegeId == collegeId &&
                            s.Status == StudentStatus.Pending)
                .Include(s => s.User)
                .Include(s => s.College)
                .Include(s => s.PlacementOfficer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.College)
                .Include(s => s.PlacementOfficer)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.College)
                .Include(s => s.PlacementOfficer)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<bool> RegisterNumberExistsAsync(
            int collegeId,
            string registerNumber)
        {
            return await _context.Students.AnyAsync(s =>
                s.CollegeId == collegeId &&
                s.RegisterNumber == registerNumber);
        }

        public async Task<PlacementOfficer?> GetPlacementOfficerByUserIdAsync(
            string userId)
        {
            return await _context.PlacementOfficers
                .Include(p => p.User)
                .Include(p => p.College)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
