using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ViemoServices
{
    public class VimeoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _vimeoAccessToken;
        private readonly string _allowedDomain;

        public VimeoService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _vimeoAccessToken = config["Vimeo:AccessToken"];
            _allowedDomain = config["Vimeo:AllowedDomain"];
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _vimeoAccessToken);
        }

        public async Task<string> UploadVideoAsync(string filePath, string title)
        {
            // 1. Upload the video file
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var content = new ByteArrayContent(fileBytes);
            content.Headers.Add("Content-Type", "video/mp4");

            var uploadResponse = await _httpClient.PostAsync("https://api.vimeo.com/me/videos", content);
            if (!uploadResponse.IsSuccessStatusCode)
                throw new Exception("Vimeo upload failed: " + await uploadResponse.Content.ReadAsStringAsync());

            var json = await uploadResponse.Content.ReadAsStringAsync();
            var vimeoData = JsonSerializer.Deserialize<JsonElement>(json);
            var videoId = vimeoData.GetProperty("uri").GetString()?.Split("/").Last();

            // 2. Set privacy and disable download
            var patchBody = new
            {
                name = title,
                privacy = new
                {
                    view = "unlisted",
                    download = false,
                    embed = "whitelist"
                },
                embed = new { buttons = new { fullscreen = true } } // Optional UI customization
            };
            var patchJson = new StringContent(JsonSerializer.Serialize(patchBody), Encoding.UTF8, "application/json");

            var patchResponse = await _httpClient.PatchAsync($"https://api.vimeo.com/videos/{videoId}", patchJson);
            if (!patchResponse.IsSuccessStatusCode)
                throw new Exception("Vimeo privacy update failed: " + await patchResponse.Content.ReadAsStringAsync());

            // 3. Whitelist your domain (optional)
            var domainBody = new { domain = _allowedDomain };
            var domainJson = new StringContent(JsonSerializer.Serialize(domainBody), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"https://api.vimeo.com/videos/{videoId}/privacy/domains", domainJson);

            // 4. Return embed link
            return $"https://player.vimeo.com/video/{videoId}";
        }
    }

}
