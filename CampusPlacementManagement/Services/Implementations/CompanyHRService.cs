
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.CompanyHR;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services.Implementations
{
    public class CompanyHRService : ICompanyHRService
    {
        private readonly ICompanyHRRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyRepository _companyRepository;

        public CompanyHRService(
            ICompanyHRRepository repository,
            UserManager<ApplicationUser> userManager,
            ICompanyRepository companyRepository)
        {
            _repository = repository;
            _userManager = userManager;
            _companyRepository = companyRepository;
        }

        // Company HR registration
        public async Task<CompanyHRResponseDto> RegisterAsync(
            CompanyHRRegisterDto request)
        {
            // Check whether the company exists.
            var company = await _companyRepository
                .GetByIdAsync(request.CompanyId);

            if (company == null)
            {
                throw new InvalidOperationException(
                    "Company not found.");
            }

            // Only approved companies can register HR users.
            if (company.Status != CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Company must be approved before registering HR users.");
            }

            // Check whether the email already exists.
            var existingUser = await _userManager
                .FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                AccountStatus = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userResult = await _userManager
                .CreateAsync(user, request.Password);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    userResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            // Assign CompanyHR role.
            var roleResult = await _userManager
                .AddToRoleAsync(user, UserRoles.CompanyHR);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            var companyHR = new CompanyHR
            {
                UserId = user.Id,
                CompanyId = request.CompanyId,
                Designation = request.Designation
            };

            await _repository.AddAsync(companyHR);

            var savedHR = await _repository
                .GetByUserIdAsync(user.Id);

            return MapToResponse(savedHR!);
        }

        // SuperAdmin: Get all HR users.
        public async Task<List<CompanyHRResponseDto>> GetAllAsync()
        {
            var companyHRs = await _repository.GetAllAsync();

            return companyHRs
                .Select(MapToResponse)
                .ToList();
        }

        // SuperAdmin: Get pending HR users.
        public async Task<List<CompanyHRResponseDto>> GetPendingAsync()
        {
            var companyHRs = await _repository.GetPendingAsync();

            return companyHRs
                .Select(MapToResponse)
                .ToList();
        }

        // SuperAdmin: Approve HR account.
        public async Task<CompanyHRResponseDto?> ApproveAsync(
            int id, string approvedBy)
        {
            var companyHR = await _repository.GetByIdAsync(id);

            if (companyHR == null)
                return null;

            if (companyHR.User.AccountStatus != AccountStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending Company HR accounts can be approved.");
            }

            companyHR.User.AccountStatus = AccountStatus.Approved;
            companyHR.User.UpdatedAt = DateTime.UtcNow;

            companyHR.ApprovedBy = approvedBy;
            companyHR.ApprovedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(companyHR.User);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            await _repository.SaveChangesAsync();

            return MapToResponse(companyHR);
        }

        // SuperAdmin: Reject HR account.
        public async Task<CompanyHRResponseDto?> RejectAsync(int id)
        {
            var companyHR = await _repository.GetByIdAsync(id);

            if (companyHR == null)
                return null;

            if (companyHR.User.AccountStatus != AccountStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending Company HR accounts can be rejected.");
            }

            companyHR.User.AccountStatus = AccountStatus.Rejected;
            companyHR.User.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(companyHR.User);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            await _repository.SaveChangesAsync();

            return MapToResponse(companyHR);
        }

        // CompanyHR: Get own profile.
        public async Task<CompanyHRResponseDto?> GetMyProfileAsync(
            string userId)
        {
            var companyHR = await _repository.GetByUserIdAsync(userId);

            if (companyHR == null)
                return null;

            return MapToResponse(companyHR);
        }

        private static CompanyHRResponseDto MapToResponse(
            CompanyHR companyHR)
        {
            return new CompanyHRResponseDto
            {
                CompanyHRId = companyHR.CompanyHRId,
                UserId = companyHR.UserId,
                FullName = companyHR.User.FullName,
                Email = companyHR.User.Email ?? string.Empty,
                PhoneNumber = companyHR.User.PhoneNumber ?? string.Empty,
                CompanyId = companyHR.CompanyId,
                CompanyName = companyHR.Company.CompanyName,
                Designation = companyHR.Designation,
                AccountStatus = companyHR.User.AccountStatus.ToString(),
                ApprovedBy = companyHR.ApprovedBy,
                ApprovedAt = companyHR.ApprovedAt
            };
        }
    }
}
