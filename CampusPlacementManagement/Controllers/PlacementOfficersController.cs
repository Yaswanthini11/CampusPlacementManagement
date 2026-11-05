using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.PlacementOfficer;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacementOfficersController : ControllerBase
    {
        private readonly IPlacementOfficerService _service;

        public PlacementOfficersController(
            IPlacementOfficerService service)
        {
            _service = service;
        }

        // PUBLIC: Placement Officer registration
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<PlacementOfficerResponseDto>> Register(
            PlacementOfficerRegisterDto request)
        {
            try
            {
                var result = await _service.RegisterAsync(request);

                return CreatedAtAction(
                    nameof(GetOfficerById),
                    new { userId = result.UserId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SUPERADMIN: Get all Placement Officers
        [HttpGet]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<List<PlacementOfficerResponseDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        // SUPERADMIN: Get pending Placement Officers
        [HttpGet("pending")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<List<PlacementOfficerResponseDto>>> GetPending()
        {
            var result = await _service.GetPendingAsync();

            return Ok(result);
        }

        // SUPERADMIN: Get Placement Officer by ID
        [HttpGet("{userId}")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<PlacementOfficerResponseDto>> GetOfficerById(
            string userId)
        {
            var result = await _service.GetByIdAsync(userId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Placement Officer not found."
                });
            }

            return Ok(result);
        }

        // SUPERADMIN: Approve Placement Officer
        [HttpPatch("{userId}/approve")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<PlacementOfficerResponseDto>> Approve(
            string userId)
        {
            try
            {
                string? adminId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized();
                }

                var result =
                    await _service.ApproveAsync(userId, adminId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Placement Officer not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // SUPERADMIN: Reject Placement Officer
        [HttpPatch("{userId}/reject")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<PlacementOfficerResponseDto>> Reject(
            string userId)
        {
            try
            {
                var result = await _service.RejectAsync(userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Placement Officer not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // SUPERADMIN: Suspend Placement Officer
        [HttpPatch("{userId}/suspend")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<PlacementOfficerResponseDto>> Suspend(
            string userId)
        {
            try
            {
                var result = await _service.SuspendAsync(userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Placement Officer not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PLACEMENT OFFICER: Get own profile
        [HttpGet("me")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<ActionResult<PlacementOfficerResponseDto>> GetMyProfile()
        {
            string? userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _service.GetMyProfileAsync(userId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Placement Officer profile not found."
                });
            }

            return Ok(result);
        }
    }
}