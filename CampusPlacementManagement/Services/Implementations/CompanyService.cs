using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyResponseDto> RegisterAsync(
            CompanyRegisterDto request)
        {
            string email = request.CompanyEmail.Trim();

            bool emailExists =
                await _repository.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A company with this email already exists.");
            }

            var company = new Company
            {
                CompanyName = request.CompanyName.Trim(),
                CompanyEmail = email,
                Website = request.Website?.Trim(),
                Industry = request.Industry.Trim(),
                Address = request.Address.Trim(),
                Description = request.Description?.Trim(),
                Status = CompanyStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(company);
            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        public async Task<List<CompanyResponseDto>> GetAllAsync()
        {
            var companies = await _repository.GetAllAsync();

            return companies.Select(MapToResponse).ToList();
        }

        public async Task<List<CompanyResponseDto>> GetPendingAsync()
        {
            var companies = await _repository.GetPendingAsync();

            return companies.Select(MapToResponse).ToList();
        }

        public async Task<CompanyResponseDto?> GetByIdAsync(
            int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);

            return company == null
                ? null
                : MapToResponse(company);
        }

        public async Task<CompanyResponseDto?> UpdateAsync(
            int companyId,
            CompanyUpdateDto request)
        {
            var company = await _repository.GetByIdAsync(companyId);

            if (company == null)
                return null;

            string email = request.CompanyEmail.Trim();

            bool emailExists =
                await _repository.EmailExistsAsync(email);

            if (emailExists &&
                !string.Equals(
                    company.CompanyEmail,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Another company is already registered with this email.");
            }

            company.CompanyName = request.CompanyName.Trim();
            company.CompanyEmail = email;
            company.Website = request.Website?.Trim();
            company.Industry = request.Industry.Trim();
            company.Address = request.Address.Trim();
            company.Description = request.Description?.Trim();

            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        public async Task<CompanyResponseDto?> ApproveAsync(
            int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);

            if (company == null)
                return null;

            if (company.Status != CompanyStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending companies can be approved.");
            }

            company.Status = CompanyStatus.Approved;

            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        public async Task<CompanyResponseDto?> RejectAsync(
            int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);

            if (company == null)
                return null;

            if (company.Status != CompanyStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending companies can be rejected.");
            }

            company.Status = CompanyStatus.Rejected;

            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        public async Task<CompanyResponseDto?> SuspendAsync(
            int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);

            if (company == null)
                return null;

            if (company.Status != CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved companies can be suspended.");
            }

            company.Status = CompanyStatus.Suspended;

            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        private static CompanyResponseDto MapToResponse(
            Company company)
        {
            return new CompanyResponseDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyEmail = company.CompanyEmail,
                Website = company.Website,
                Industry = company.Industry,
                Address = company.Address,
                Description = company.Description,
                Status = company.Status,
                CreatedAt = company.CreatedAt
            };
        }
    }
}