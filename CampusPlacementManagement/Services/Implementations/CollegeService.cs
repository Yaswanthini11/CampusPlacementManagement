using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services.Implementations
{
    public class CollegeService : ICollegeService
    {
        private readonly ICollegeRepository _collegeRepository;

        public CollegeService(ICollegeRepository collegeRepository)
        {
            _collegeRepository = collegeRepository;
        }

        // Register a new college
        public async Task<CollegeResponseDto> RegisterCollegeAsync(
            CollegeRegisterDto request)
        {
            string collegeCode = request.CollegeCode.Trim().ToUpperInvariant();

            bool exists =
                await _collegeRepository.CollegeCodeExistsAsync(collegeCode);

            if (exists)
            {
                throw new InvalidOperationException(
                    "A college with this college code already exists.");
            }

            var college = new College
            {
                CollegeName = request.CollegeName.Trim(),
                CollegeCode = collegeCode,
                UniversityName = request.UniversityName?.Trim(),
                OfficialEmail = request.OfficialEmail.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address.Trim(),
                Website = request.Website?.Trim(),
                Status = CollegeStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _collegeRepository.AddAsync(college);
            await _collegeRepository.SaveChangesAsync();

            return MapToResponse(college);
        }

        // Get all colleges
        public async Task<List<CollegeResponseDto>> GetAllCollegesAsync()
        {
            var colleges = await _collegeRepository.GetAllAsync();

            return colleges.Select(MapToResponse).ToList();
        }

        // Get colleges awaiting approval
        public async Task<List<CollegeResponseDto>> GetPendingCollegesAsync()
        {
            var colleges =
                await _collegeRepository.GetPendingCollegesAsync();

            return colleges.Select(MapToResponse).ToList();
        }

        // Get college by ID
        public async Task<CollegeResponseDto?> GetCollegeByIdAsync(
            int collegeId)
        {
            var college =
                await _collegeRepository.GetByIdAsync(collegeId);

            return college == null ? null : MapToResponse(college);
        }

        // Update college details
        public async Task<CollegeResponseDto?> UpdateCollegeAsync(
            int collegeId,
            CollegeUpdateDto request)
        {
            var college =
                await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
            {
                return null;
            }

            string collegeCode = request.CollegeCode.Trim().ToUpperInvariant();

            bool exists =
                await _collegeRepository.CollegeCodeExistsAsync(
                    collegeCode,
                    collegeId);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Another college already uses this college code.");
            }

            college.CollegeName = request.CollegeName.Trim();
            college.CollegeCode = collegeCode;
            college.UniversityName = request.UniversityName?.Trim();
            college.OfficialEmail = request.OfficialEmail.Trim();
            college.PhoneNumber = request.PhoneNumber.Trim();
            college.Address = request.Address.Trim();
            college.Website = request.Website?.Trim();

            await _collegeRepository.SaveChangesAsync();

            return MapToResponse(college);
        }

        // Approve college
        public async Task<CollegeResponseDto?> ApproveCollegeAsync(
            int collegeId)
        {
            return await ChangeStatusAsync(
                collegeId,
                CollegeStatus.Approved);
        }

        // Reject college
        public async Task<CollegeResponseDto?> RejectCollegeAsync(
            int collegeId)
        {
            return await ChangeStatusAsync(
                collegeId,
                CollegeStatus.Rejected);
        }

        // Suspend college
        public async Task<CollegeResponseDto?> SuspendCollegeAsync(
            int collegeId)
        {
            return await ChangeStatusAsync(
                collegeId,
                CollegeStatus.Suspended);
        }

        // Common status update method
        private async Task<CollegeResponseDto?> ChangeStatusAsync(
            int collegeId,
            CollegeStatus status)
        {
            var college =
                await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
            {
                return null;
            }

            college.Status = status;

            await _collegeRepository.SaveChangesAsync();

            return MapToResponse(college);
        }

        // Convert model to response DTO
        private static CollegeResponseDto MapToResponse(College college)
        {
            return new CollegeResponseDto
            {
                CollegeId = college.CollegeId,
                CollegeName = college.CollegeName,
                CollegeCode = college.CollegeCode,
                UniversityName = college.UniversityName,
                OfficialEmail = college.OfficialEmail,
                PhoneNumber = college.PhoneNumber,
                Address = college.Address,
                Website = college.Website,
                Status = college.Status,
                CreatedAt = college.CreatedAt
            };
        }
    }
}