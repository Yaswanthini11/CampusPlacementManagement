using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.PlacementOfficer;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services.Implementations
{
    public class PlacementOfficerService : IPlacementOfficerService
    {
        private readonly IPlacementOfficerRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlacementOfficerService(
            IPlacementOfficerRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // Register a new Placement Officer
        public async Task<PlacementOfficerResponseDto> RegisterAsync(
            PlacementOfficerRegisterDto request)
        {
            bool collegeExists =
                await _repository.CollegeExistsAndApprovedAsync(
                    request.CollegeId);

            if (!collegeExists)
            {
                throw new InvalidOperationException(
                    "The selected college does not exist or is not approved.");
            }

            string email = request.Email.Trim();

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                AccountStatus = AccountStatus.Pending,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult =
                await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    GetIdentityErrors(createResult));
            }

            try
            {
                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        UserRoles.PlacementOfficer);

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        GetIdentityErrors(roleResult));
                }

                var officer = new PlacementOfficer
                {
                    UserId = user.Id,
                    CollegeId = request.CollegeId,
                    Designation = request.Designation.Trim(),
                    Status = PlacementOfficerStatus.Pending
                };

                await _repository.AddAsync(officer);
                await _repository.SaveChangesAsync();

                var savedOfficer =
                    await _repository.GetByIdAsync(user.Id);

                return MapToResponse(savedOfficer!);
            }
            catch
            {
                // Remove the Identity user if profile creation fails.
                await _userManager.DeleteAsync(user);
                throw;
            }
        }

        // Get all Placement Officers
        public async Task<List<PlacementOfficerResponseDto>> GetAllAsync()
        {
            var officers = await _repository.GetAllAsync();

            return officers.Select(MapToResponse).ToList();
        }

        // Get pending Placement Officers
        public async Task<List<PlacementOfficerResponseDto>> GetPendingAsync()
        {
            var officers = await _repository.GetPendingAsync();

            return officers.Select(MapToResponse).ToList();
        }

        // Get officer by ID
        public async Task<PlacementOfficerResponseDto?> GetByIdAsync(
            string userId)
        {
            var officer = await _repository.GetByIdAsync(userId);

            return officer == null ? null : MapToResponse(officer);
        }

        // Approve Placement Officer
        public async Task<PlacementOfficerResponseDto?> ApproveAsync(
            string userId,
            string approvedByUserId)
        {
            var officer = await _repository.GetByIdAsync(userId);

            if (officer == null)
            {
                return null;
            }

            if (officer.Status != PlacementOfficerStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending Placement Officers can be approved.");
            }

            officer.Status = PlacementOfficerStatus.Approved;
            officer.ApprovedBy = approvedByUserId;
            officer.ApprovedAt = DateTime.UtcNow;

            officer.User.AccountStatus = AccountStatus.Approved;
            officer.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(officer.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    GetIdentityErrors(updateResult));
            }

            await _repository.SaveChangesAsync();

            return MapToResponse(officer);
        }

        // Reject Placement Officer
        public async Task<PlacementOfficerResponseDto?> RejectAsync(
            string userId)
        {
            var officer = await _repository.GetByIdAsync(userId);

            if (officer == null)
            {
                return null;
            }

            if (officer.Status != PlacementOfficerStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending Placement Officers can be rejected.");
            }

            officer.Status = PlacementOfficerStatus.Rejected;
            officer.User.AccountStatus = AccountStatus.Rejected;
            officer.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(officer.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    GetIdentityErrors(updateResult));
            }

            await _repository.SaveChangesAsync();

            return MapToResponse(officer);
        }

        // Suspend Placement Officer
        public async Task<PlacementOfficerResponseDto?> SuspendAsync(
            string userId)
        {
            var officer = await _repository.GetByIdAsync(userId);

            if (officer == null)
            {
                return null;
            }

            if (officer.Status != PlacementOfficerStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved Placement Officers can be suspended.");
            }

            officer.Status = PlacementOfficerStatus.Suspended;
            officer.User.AccountStatus = AccountStatus.Suspended;
            officer.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(officer.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    GetIdentityErrors(updateResult));
            }

            await _repository.SaveChangesAsync();

            return MapToResponse(officer);
        }

        // Get the logged-in officer's own profile
        public async Task<PlacementOfficerResponseDto?> GetMyProfileAsync(
            string userId)
        {
            var officer = await _repository.GetByIdAsync(userId);

            return officer == null ? null : MapToResponse(officer);
        }

        // Convert Identity errors into readable messages
        private static string GetIdentityErrors(IdentityResult result)
        {
            return string.Join(
                "; ",
                result.Errors.Select(error => error.Description));
        }

        // Convert model into response DTO
        private static PlacementOfficerResponseDto MapToResponse(
            PlacementOfficer officer)
        {
            return new PlacementOfficerResponseDto
            {
                UserId = officer.UserId,
                FullName = officer.User.FullName,
                Email = officer.User.Email ?? string.Empty,
                PhoneNumber = officer.User.PhoneNumber ?? string.Empty,
                Designation = officer.Designation,
                CollegeId = officer.CollegeId,
                CollegeName = officer.College.CollegeName,
                Status = officer.Status,
                ApprovedBy = officer.ApprovedBy,
                ApprovedAt = officer.ApprovedAt
            };
        }
    }
}