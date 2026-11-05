
using System.Security.Claims;
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.CompanyHR;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/company-hrs")]
    [ApiController]
    public class CompanyHRsController : ControllerBase
    {
        private readonly ICompanyHRService _companyHRService;

        public CompanyHRsController(
            ICompanyHRService companyHRService)
        {
            _companyHRService = companyHRService;
        }

        // Public: Company HR registration.
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            CompanyHRRegisterDto request)
        {
            try
            {
                var result =
                    await _companyHRService.RegisterAsync(request);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Get all HR users.
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _companyHRService.GetAllAsync();

            return Ok(result);
        }

        // SuperAdmin: Get pending HR users.
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _companyHRService.GetPendingAsync();

            return Ok(result);
        }

        // SuperAdmin: Approve HR account.
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var approvedBy = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(approvedBy))
                    return Unauthorized();

                var result = await _companyHRService
                    .ApproveAsync(id, approvedBy);

                if (result == null)
                {
                    return NotFound(
                        new { message = "Company HR not found." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SuperAdmin: Reject HR account.
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result =
                    await _companyHRService.RejectAsync(id);

                if (result == null)
                {
                    return NotFound(
                        new { message = "Company HR not found." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // CompanyHR: Get own profile.
        [Authorize(Roles = UserRoles.CompanyHR)]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result =
                await _companyHRService.GetMyProfileAsync(userId);

            if (result == null)
            {
                return NotFound(
                    new { message = "Company HR profile not found." });
            }

            return Ok(result);
        }
    }
}
