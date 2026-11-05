
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Auth;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusPlacementManagement.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto request)
        {
            string email = request.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            bool passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    request.Password);

            if (!passwordValid)
                return null;

            // Only approved accounts can log in.
            if (user.AccountStatus != AccountStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Your account is not approved or is inactive.");
            }

            // Get the user's assigned role.
            var roles = await _userManager.GetRolesAsync(user);

            string? userRole = roles.FirstOrDefault(role =>
    role == UserRoles.SuperAdmin ||
    role == UserRoles.PlacementOfficer ||
    role == UserRoles.CompanyHR ||
    role == UserRoles.Student);
            // Check whether the role is allowed to log in.
            if (userRole == null)
            {
                throw new InvalidOperationException(
                    "You are not authorized to log in.");
            }

            // Read JWT configuration.
            string key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key is not configured.");

            string issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer is not configured.");

            string audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is not configured.");

            // Create JWT claims.
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName ?? email),

                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? email),

                new Claim(
                    ClaimTypes.Role,
                    userRole)
            };

            // Create signing key.
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            // Token expires after two hours.
            DateTime expiresAt = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            string tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            // Return login response.
            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? email,
                Role = userRole,
                Token = tokenString,
                ExpiresAt = expiresAt
            };
        }
    }
}
