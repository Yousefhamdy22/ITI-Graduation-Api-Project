
using Core.Entities.Identity;
using Infrastructure.Helper;
using System;
namespace Infrastructure.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResult> GenerateTokenAsync(ApplicationUser user);

        Task<AuthResult> GenerateRefreshTokenAsync(string token, string refreshToken);

        Task<bool> RevokeTokenAsync(string refreshToken);

        Task<ApplicationUser> GetUserFromTokenAsync(string token);





    }
}