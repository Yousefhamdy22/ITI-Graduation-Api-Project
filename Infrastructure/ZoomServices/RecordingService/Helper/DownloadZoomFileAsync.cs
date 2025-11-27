
using System.Net.Http.Headers;


namespace Infrastructure.ZoomServices.RecordingService.Helper
{
    public class DownloadZoomFileAsyncwithAuth
    {

        private readonly IZoomAuthService _zoomAuthService;
        public DownloadZoomFileAsyncwithAuth(IZoomAuthService zoomAuthService)
        {  
            _zoomAuthService = zoomAuthService;
        }

        public async Task<string> DownloadZoomFileAsync(
                                                          string fileUrl,
                                                          string recordingId,
                                                          string fileType,
                                                          CancellationToken ct)
        {
            // Save in temp folder
            var localPath = Path.Combine(Path.GetTempPath(), $"{recordingId}.{fileType}");

            using var client = new HttpClient();
            var _zoomJwtToken = _zoomAuthService.GetAccessTokenAsync(ct).Result;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _zoomJwtToken); 

            using var response = await client.GetAsync(fileUrl, ct);
            response.EnsureSuccessStatusCode();

            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs, ct);

            return localPath;
        }

    }
}
