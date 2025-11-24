using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Helper
{
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
    public class ZoomSettings
    {
        [JsonPropertyName("AccountId")]
        public string AccountId { get; set; } = string.Empty;

        [JsonPropertyName("ClientId")]
        public string ClientId { get; set; } = string.Empty;

        [JsonPropertyName("ClientSecret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("BaseUrl")]
        public string BaseUrl { get; set; } = "https://api.zoom.us/v2";

        [JsonPropertyName("WebhookSecret")]
        public string WebhookSecret { get; set; } = string.Empty;

        [JsonPropertyName("DefaultUserId")]
        public string? DefaultUserId { get; set; }

        [JsonPropertyName("DefaultTimezone")]
        public string DefaultTimezone { get; set; } = "UTC";
    }

}
