namespace Core.DTOs;

public class JwtSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
}

public class AuthResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string SessionId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; }
}