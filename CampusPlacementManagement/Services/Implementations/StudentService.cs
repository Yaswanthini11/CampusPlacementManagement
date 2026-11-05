
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(
            IStudentRepository studentRepository,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        public async Task<StudentResponseDto> EnrollAsync(
            StudentEnrollDto request,
            string placementOfficerUserId)
        {
            var officer =
                await _studentRepository.GetPlacementOfficerByUserIdAsync(
                    placementOfficerUserId);

            if (officer == null ||
                officer.Status != PlacementOfficerStatus.Approved ||
                officer.User.AccountStatus != AccountStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only an approved Placement Officer can enroll students.");
            }

            if (officer.College.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Students cannot be enrolled under an unapproved college.");
            }

            if (await _studentRepository.RegisterNumberExistsAsync(
                officer.CollegeId,
                request.RegisterNumber.Trim()))
            {
                throw new InvalidOperationException(
                    "This register number already exists in your college.");
            }

            if (await _userManager.FindByEmailAsync(request.Email.Trim()) != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email.Trim(),
                Email = request.Email.Trim(),
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                AccountStatus = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(
                user,
                request.InitialPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                UserRoles.Student);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                throw new InvalidOperationException(
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }

            var student = new Student
            {
                UserId = user.Id,
                CollegeId = officer.CollegeId,
                PlacementOfficerUserId = officer.UserId,
                RegisterNumber = request.RegisterNumber.Trim(),
                Department = request.Department.Trim(),
                Degree = request.Degree.Trim(),
                GraduationYear = request.GraduationYear,
                CGPA = request.CGPA,
                ActiveBacklogs = request.ActiveBacklogs,
                ResumeUrl = request.ResumeUrl,
                Status = StudentStatus.Pending
            };

            try
            {
                await _studentRepository.AddAsync(student);
                await _studentRepository.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            var savedStudent =
                await _studentRepository.GetByIdAsync(student.StudentId);

            return MapToResponse(savedStudent!);
        }

        public async Task<List<StudentResponseDto>> BulkEnrollAsync(
            BulkStudentEnrollDto request,
            string placementOfficerUserId)
        {
            var results = new List<StudentResponseDto>();

            foreach (var studentRequest in request.Students)
            {
                var result = await EnrollAsync(
                    studentRequest,
                    placementOfficerUserId);

                results.Add(result);
            }

            return results;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync(
            string userId,
            string role)
        {
            List<Student> students;

            if (role == UserRoles.SuperAdmin)
            {
                students = await _studentRepository.GetAllAsync();
            }
            else if (role == UserRoles.PlacementOfficer)
            {
                var officer =
                    await _studentRepository.GetPlacementOfficerByUserIdAsync(
                        userId);

                if (officer == null ||
                    officer.Status != PlacementOfficerStatus.Approved ||
                    officer.College.Status != CollegeStatus.Approved)
                {
                    throw new InvalidOperationException(
                        "Your Placement Officer account or college is not active.");
                }

                students = await _studentRepository.GetByCollegeIdAsync(
                    officer.CollegeId);
            }
            else
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view all students.");
            }

            return students.Select(MapToResponse).ToList();
        }

        public async Task<List<StudentResponseDto>> GetPendingAsync(
            string placementOfficerUserId)
        {
            var officer =
                await _studentRepository.GetPlacementOfficerByUserIdAsync(
                    placementOfficerUserId);

            if (officer == null ||
                officer.Status != PlacementOfficerStatus.Approved ||
                officer.College.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Your Placement Officer account or college is not active.");
            }

            var students =
                await _studentRepository.GetPendingByCollegeIdAsync(
                    officer.CollegeId);

            return students.Select(MapToResponse).ToList();
        }

        public async Task<StudentResponseDto?> GetByIdAsync(
            int studentId,
            string userId,
            string role)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                return null;

            if (role == UserRoles.SuperAdmin)
                return MapToResponse(student);

            if (role == UserRoles.PlacementOfficer)
            {
                var officer =
                    await _studentRepository.GetPlacementOfficerByUserIdAsync(
                        userId);

                if (officer == null ||
                    officer.Status != PlacementOfficerStatus.Approved ||
                    officer.College.Status != CollegeStatus.Approved ||
                    officer.CollegeId != student.CollegeId)
                {
                    throw new UnauthorizedAccessException(
                        "You cannot access students from another college.");
                }

                return MapToResponse(student);
            }

            if (role == UserRoles.Student &&
                student.UserId == userId)
            {
                return MapToResponse(student);
            }

            throw new UnauthorizedAccessException(
                "You are not authorized to view this student.");
        }

        public async Task<StudentResponseDto?> ApproveAsync(
            int studentId,
            string placementOfficerUserId)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                return null;

            var officer =
                await _studentRepository.GetPlacementOfficerByUserIdAsync(
                    placementOfficerUserId);

            ValidateOfficerAccess(officer, student);

            if (student.Status != StudentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending students can be approved.");
            }

            student.Status = StudentStatus.Approved;
            student.ApprovedAt = DateTime.UtcNow;

            student.User.AccountStatus = AccountStatus.Approved;
            student.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(student.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            await _studentRepository.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<StudentResponseDto?> RejectAsync(
            int studentId,
            string placementOfficerUserId)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                return null;

            var officer =
                await _studentRepository.GetPlacementOfficerByUserIdAsync(
                    placementOfficerUserId);

            ValidateOfficerAccess(officer, student);

            if (student.Status != StudentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending students can be rejected.");
            }

            student.Status = StudentStatus.Rejected;
            student.User.AccountStatus = AccountStatus.Rejected;
            student.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(student.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            await _studentRepository.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<StudentResponseDto?> SuspendAsync(
            int studentId,
            string userId,
            string role)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                return null;

            if (role == UserRoles.PlacementOfficer)
            {
                var officer =
                    await _studentRepository.GetPlacementOfficerByUserIdAsync(
                        userId);

                ValidateOfficerAccess(officer, student);
            }
            else if (role != UserRoles.SuperAdmin)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to suspend students.");
            }

            if (student.Status == StudentStatus.Suspended)
            {
                throw new InvalidOperationException(
                    "This student is already suspended.");
            }

            student.Status = StudentStatus.Suspended;
            student.User.AccountStatus = AccountStatus.Suspended;
            student.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(student.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            await _studentRepository.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<StudentResponseDto?> UpdateAsync(
            int studentId,
            StudentUpdateDto request,
            string userId,
            string role)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                return null;

            bool isOwnProfile =
                role == UserRoles.Student && student.UserId == userId;

            if (role == UserRoles.PlacementOfficer)
            {
                var officer =
                    await _studentRepository.GetPlacementOfficerByUserIdAsync(
                        userId);

                ValidateOfficerAccess(officer, student);
            }
            else if (role != UserRoles.SuperAdmin && !isOwnProfile)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to update this student.");
            }

            if (isOwnProfile)
            {
                // Students can update only their contact number and resume.
                if (request.PhoneNumber != null)
                    student.User.PhoneNumber = request.PhoneNumber.Trim();

                if (request.ResumeUrl != null)
                    student.ResumeUrl = request.ResumeUrl;
            }
            else
            {
                if (request.PhoneNumber != null)
                    student.User.PhoneNumber = request.PhoneNumber.Trim();

                if (request.Department != null)
                    student.Department = request.Department.Trim();

                if (request.Degree != null)
                    student.Degree = request.Degree.Trim();

                if (request.GraduationYear.HasValue)
                    student.GraduationYear = request.GraduationYear.Value;

                if (request.CGPA.HasValue)
                    student.CGPA = request.CGPA.Value;

                if (request.ActiveBacklogs.HasValue)
                    student.ActiveBacklogs = request.ActiveBacklogs.Value;

                if (request.ResumeUrl != null)
                    student.ResumeUrl = request.ResumeUrl;
            }

            student.User.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(student.User);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            await _studentRepository.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<StudentResponseDto?> GetMyProfileAsync(string userId)
        {
            var student =
                await _studentRepository.GetByUserIdAsync(userId);

            return student == null ? null : MapToResponse(student);
        }

        private static void ValidateOfficerAccess(
            PlacementOfficer? officer,
            Student student)
        {
            if (officer == null ||
                officer.Status != PlacementOfficerStatus.Approved ||
                officer.User.AccountStatus != AccountStatus.Approved ||
                officer.College.Status != CollegeStatus.Approved ||
                officer.CollegeId != student.CollegeId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot manage students outside your approved college.");
            }
        }

        private static StudentResponseDto MapToResponse(Student student)
        {
            return new StudentResponseDto
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                FullName = student.User.FullName,
                Email = student.User.Email ?? string.Empty,
                PhoneNumber = student.User.PhoneNumber ?? string.Empty,
                CollegeId = student.CollegeId,
                CollegeName = student.College.CollegeName,
                PlacementOfficerUserId = student.PlacementOfficerUserId,
                RegisterNumber = student.RegisterNumber,
                Department = student.Department,
                Degree = student.Degree,
                GraduationYear = student.GraduationYear,
                CGPA = student.CGPA,
                ActiveBacklogs = student.ActiveBacklogs,
                ResumeUrl = student.ResumeUrl,
                Status = student.Status.ToString(),
                ApprovedAt = student.ApprovedAt,
                CreatedAt = student.User.CreatedAt
            };
        }
    }
}
