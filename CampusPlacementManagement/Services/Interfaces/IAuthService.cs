using CampusPlacementManagement.DTOs.Auth;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto request);
    }
}