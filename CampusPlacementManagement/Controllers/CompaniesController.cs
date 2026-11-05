using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // Public company registration

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            CompanyRegisterDto request)
        {
            try
            {
                var result =
                    await _companyService.RegisterAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.CompanyId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Get all companies

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _companyService.GetAllAsync();

            return Ok(result);
        }

        // SuperAdmin: Get pending companies

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _companyService.GetPendingAsync();

            return Ok(result);
        }

        // SuperAdmin: Get company by ID

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _companyService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Company not found." });

            return Ok(result);
        }

        // SuperAdmin: Update company

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CompanyUpdateDto request)
        {
            try
            {
                var result =
                    await _companyService.UpdateAsync(id, request);

                if (result == null)
                    return NotFound(new { message = "Company not found." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Approve company

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var result = await _companyService.ApproveAsync(id);

                if (result == null)
                    return NotFound(new { message = "Company not found." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Reject company

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result = await _companyService.RejectAsync(id);

                if (result == null)
                    return NotFound(new { message = "Company not found." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Suspend company

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}/suspend")]
        public async Task<IActionResult> Suspend(int id)
        {
            try
            {
                var result = await _companyService.SuspendAsync(id);

                if (result == null)
                    return NotFound(new { message = "Company not found." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}