using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ILogger<RefreshTokenRepository> _logger;
    private readonly AppDBContext _context;

    public RefreshTokenRepository(AppDBContext context , ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiration)
    {
        var token = new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresOnUtc = expiration,
            CreatedAtUtc = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Token == refreshToken &&
                !x.IsRevoked &&
                x.ExpiresOnUtc > DateTime.UtcNow);

        return token != null;
    }


    public async Task RevokeRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<RefreshToken> GetTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);
    }
}