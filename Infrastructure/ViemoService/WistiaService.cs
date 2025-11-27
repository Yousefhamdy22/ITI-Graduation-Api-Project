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

namespace Infrastructure.ViemoService
{
    public class WistiaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly string _projectId;
        private readonly string _uploadUrl;
        private readonly string _allowedDomain;

        public WistiaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiToken = config["Wistia:ApiToken"];
            _projectId = config["Wistia:ProjectId"];
            _uploadUrl = config["Wistia:UploadUrl"];
            _allowedDomain = config["Wistia:AllowedDomain"];
        }

        public async Task<string> UploadVideoAsync(string filePath, string title)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Video file not found.", filePath);

            // 1️⃣ Upload to Wistia
            using var form = new MultipartFormDataContent
        {
            { new StringContent(_apiToken), "api_password" },
            { new StringContent(title), "name" },
            { new StringContent(_projectId), "project_id" }
        };

            var fileContent = new StreamContent(File.OpenRead(filePath));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync(_uploadUrl, form);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Wistia upload failed: {json}");

            var data = JsonSerializer.Deserialize<JsonElement>(json);
            var hashedId = data.GetProperty("hashed_id").GetString();

            // 2️⃣ Update video settings (privacy, domain)
            await ConfigureVideoPrivacyAsync(hashedId);

            // 3️⃣ Return Wistia embed URL
            return $"https://fast.wistia.net/embed/iframe/{hashedId}";
        }

        private async Task ConfigureVideoPrivacyAsync(string hashedId)
        {
            var url = $"https://api.wistia.com/v1/medias/{hashedId}.json?api_password={_apiToken}";

            var body = new
            {
                embed_options = new
                {
                    playerColor = "000000",
                    controlsVisibleOnLoad = true,
                    autoPlay = false
                },
                allow_download = false,
                privacy = "private"
            };

            var jsonBody = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, jsonBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to configure Wistia video privacy: {error}");
            }
        }
    }
}
