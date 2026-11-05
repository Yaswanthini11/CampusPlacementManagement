
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/students")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found.");

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("User role not found.");

        [HttpPost("enroll")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<IActionResult> Enroll(StudentEnrollDto request)
        {
            try
            {
                var result = await _studentService.EnrollAsync(
                    request,
                    CurrentUserId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.StudentId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("bulk-enroll")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<IActionResult> BulkEnroll(
            BulkStudentEnrollDto request)
        {
            try
            {
                var result = await _studentService.BulkEnrollAsync(
                    request,
                    CurrentUserId);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles =
            UserRoles.SuperAdmin + "," + UserRoles.PlacementOfficer)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _studentService.GetAllAsync(
                    CurrentUserId,
                    CurrentRole);

                return Ok(result);
            }
            catch (UnauthorizedAccessException )
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var result = await _studentService.GetPendingAsync(
                    CurrentUserId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles =
            UserRoles.SuperAdmin + "," +
            UserRoles.PlacementOfficer + "," +
            UserRoles.Student)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _studentService.GetByIdAsync(
                    id,
                    CurrentUserId,
                    CurrentRole);

                if (result == null)
                    return NotFound(new { message = "Student not found." });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var result = await _studentService.ApproveAsync(
                    id,
                    CurrentUserId);

                if (result == null)
                    return NotFound(new { message = "Student not found." });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/reject")]
        [Authorize(Roles = UserRoles.PlacementOfficer)]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result = await _studentService.RejectAsync(
                    id,
                    CurrentUserId);

                if (result == null)
                    return NotFound(new { message = "Student not found." });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/suspend")]
        [Authorize(Roles =
            UserRoles.SuperAdmin + "," + UserRoles.PlacementOfficer)]
        public async Task<IActionResult> Suspend(int id)
        {
            try
            {
                var result = await _studentService.SuspendAsync(
                    id,
                    CurrentUserId,
                    CurrentRole);

                if (result == null)
                    return NotFound(new { message = "Student not found." });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles =
            UserRoles.SuperAdmin + "," +
            UserRoles.PlacementOfficer + "," +
            UserRoles.Student)]
        public async Task<IActionResult> Update(
            int id,
            StudentUpdateDto request)
        {
            try
            {
                var result = await _studentService.UpdateAsync(
                    id,
                    request,
                    CurrentUserId,
                    CurrentRole);

                if (result == null)
                    return NotFound(new { message = "Student not found." });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetMyProfile()
        {
            var result =
                await _studentService.GetMyProfileAsync(CurrentUserId);

            if (result == null)
                return NotFound(new { message = "Student profile not found." });

            return Ok(result);
        }
    }
}
