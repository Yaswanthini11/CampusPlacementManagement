using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollegesController : ControllerBase
    {
        private readonly ICollegeService _collegeService;

        public CollegesController(ICollegeService collegeService)
        {
            _collegeService = collegeService;
        }

        // PUBLIC: Register a college
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<CollegeResponseDto>> RegisterCollege(
            CollegeRegisterDto request)
        {
            try
            {
                var result =
                    await _collegeService.RegisterCollegeAsync(request);

                return CreatedAtAction(
                    nameof(GetCollegeById),
                    new { id = result.CollegeId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SUPERADMIN: Get all colleges
        [HttpGet]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<List<CollegeResponseDto>>> GetAllColleges()
        {
            var result =
                await _collegeService.GetAllCollegesAsync();

            return Ok(result);
        }

        // SUPERADMIN: Get pending colleges
        [HttpGet("pending")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<List<CollegeResponseDto>>> GetPendingColleges()
        {
            var result =
                await _collegeService.GetPendingCollegesAsync();

            return Ok(result);
        }

        // SUPERADMIN: Get college by ID
        [HttpGet("{id:int}")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<CollegeResponseDto>> GetCollegeById(
            int id)
        {
            var result =
                await _collegeService.GetCollegeByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }

        // SUPERADMIN: Update college
        [HttpPut("{id:int}")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<CollegeResponseDto>> UpdateCollege(
            int id,
            CollegeUpdateDto request)
        {
            try
            {
                var result =
                    await _collegeService.UpdateCollegeAsync(id, request);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "College not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // SUPERADMIN: Approve college
        [HttpPatch("{id:int}/approve")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<CollegeResponseDto>> ApproveCollege(
            int id)
        {
            var result =
                await _collegeService.ApproveCollegeAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }

        // SUPERADMIN: Reject college
        [HttpPatch("{id:int}/reject")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<CollegeResponseDto>> RejectCollege(
            int id)
        {
            var result =
                await _collegeService.RejectCollegeAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }

        // SUPERADMIN: Suspend college
        [HttpPatch("{id:int}/suspend")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        public async Task<ActionResult<CollegeResponseDto>> SuspendCollege(
            int id)
        {
            var result =
                await _collegeService.SuspendCollegeAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }
    }
}