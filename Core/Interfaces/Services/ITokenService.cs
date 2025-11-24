using Core.DTOs;
using Core.Entities.Identity;

namespace Core.Interfaces.Services;

public interface ITokenService
{
    Task<AuthResult> GenerateTokenAsync(ApplicationUser user);

    Task<AuthResult> GenerateRefreshTokenAsync();

    Task<bool> RevokeTokenAsync(string refreshToken);

    Task<ApplicationUser> GetUserFromTokenAsync(string token);
}