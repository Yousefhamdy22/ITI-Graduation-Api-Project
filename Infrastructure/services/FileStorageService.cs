using Core.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    // Allowed Extensions (images + docs)
    private readonly string[] _allowedExtensions =
    {
        ".jpg", ".jpeg", ".png", ".gif", // images
        ".pdf", ".doc", ".docx", ".xlsx" // documents
    };

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
        _logger = logger;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;

        try
        {
            // Determine web root (same fallback as Upload)
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                var contentRoot = string.IsNullOrEmpty(_env.ContentRootPath)
                    ? Directory.GetCurrentDirectory()
                    : _env.ContentRootPath;
                webRoot = Path.Combine(contentRoot, "wwwroot");
            }

            _logger.LogInformation("Attempting to delete file: {FilePath} (webroot={WebRoot})", filePath, webRoot);

            // If the stored path is an absolute URL, extract its path portion
            var relative = filePath;
            if (Uri.TryCreate(filePath, UriKind.Absolute, out var uri))
                relative = uri.AbsolutePath; // includes leading '/'

            // Remove query string or fragment
            var qIdx = relative.IndexOf('?');
            if (qIdx >= 0) relative = relative.Substring(0, qIdx);
            var decoded = Uri.UnescapeDataString(relative).TrimStart('/');

            // If decoded contains parent directory segments, sanitize by reducing to filename only
            if (decoded.Contains(".."))
            {
                _logger.LogWarning("DeleteFileAsync received a path with parent segments: {Path}. Using filename only.",
                    decoded);
                decoded = Path.GetFileName(decoded);
            }

            // Normalize paths that may include 'wwwroot' segment (e.g. "../wwwroot/Questions/..")
            var wwwrootIndex = decoded.IndexOf("wwwroot", StringComparison.OrdinalIgnoreCase);
            if (wwwrootIndex >= 0)
            {
                // take the part after 'wwwroot'
                decoded = decoded.Substring(wwwrootIndex + "wwwroot".Length).TrimStart('/', '\\');
                _logger.LogInformation("DeleteFileAsync normalized path to: {Decoded}", decoded);
            }

            // If there's no filename (e.g. path ends with a slash or is a folder), bail out
            var filename = Path.GetFileName(decoded);
            if (string.IsNullOrEmpty(filename))
            {
                _logger.LogInformation(
                    "DeleteFileAsync: no filename could be extracted from '{Path}', skipping deletion.", filePath);
                return Task.CompletedTask;
            }

            // If filePath is an absolute filesystem path, try to delete directly
            if (Path.IsPathRooted(filePath))
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogInformation("Deleted absolute file path: {Path}", filePath);
                        return Task.CompletedTask;
                    }

                    _logger.LogInformation("Absolute file path not found: {Path} — will try resolving under webroot",
                        filePath);
                    // don't return here; continue to try resolving under webroot
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to delete absolute file path: {Path} — will try resolving under webroot", filePath);
                    // don't return; continue with other resolution attempts
                }

            // continue to try resolving using webroot/relative methods
            // Compose candidate path under webroot
            var candidate = Path.Combine(webRoot, decoded.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(candidate))
            {
                File.Delete(candidate);
                _logger.LogInformation("Deleted file: {Candidate}", candidate);
                return Task.CompletedTask;
            }

            // If not found using the relative path, try locating by filename in webroot recursively
            if (!string.IsNullOrEmpty(filename) && Directory.Exists(webRoot))
            {
                var matches = Directory.GetFiles(webRoot, filename, SearchOption.AllDirectories);
                if (matches.Length > 0)
                {
                    foreach (var m in matches)
                        try
                        {
                            File.Delete(m);
                            _logger.LogInformation("Deleted matched file: {MatchedPath}", m);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete matched file: {MatchedPath}", m);
                        }

                    return Task.CompletedTask;
                }
            }

            _logger.LogInformation("No local file deleted for: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            // Don't throw — we want update to continue even if delete fails. Log for diagnostics.
            _logger.LogWarning(ex, "Exception while attempting to delete file: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }

    public async Task<string> UpdateFileAsync(string existingFilePath, string newFileName, Stream newFileStream,
        string newFolder)
    {
        // Upload first to ensure new file saved successfully before removing old one
        var newPath = await UploadFileAsync(newFileName, newFileStream, newFolder);

        if (!string.IsNullOrEmpty(existingFilePath))
            try
            {
                await DeleteFileAsync(existingFilePath);
            }
            catch (Exception ex)
            {
                // Log and swallow so update still succeeds even if delete fails
                _logger.LogWarning(ex, "Failed to delete existing file after upload: {ExistingFilePath}",
                    existingFilePath);
            }

        return newPath;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string folder)
    {
        // Determine the web root path the running app is using. Fall back to ContentRoot/wwwroot if necessary.
        var webRoot = _env.WebRootPath;
        if (string.IsNullOrEmpty(webRoot))
        {
            var contentRoot = string.IsNullOrEmpty(_env.ContentRootPath)
                ? Directory.GetCurrentDirectory()
                : _env.ContentRootPath;
            webRoot = Path.Combine(contentRoot, "wwwroot");
        }

        if (string.IsNullOrEmpty(webRoot))
            throw new InvalidOperationException("WebRootPath not configured.");

        // 🟢 Validate extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException("File type is not allowed.");

        // 🟢 Validate file size
        try
        {
            // If the incoming stream supports length, check it, otherwise skip this strict check.
            if (fileStream.CanSeek && fileStream.Length > MaxFileSize)
                throw new InvalidOperationException("File size exceeds the 5MB limit.");
        }
        catch
        {
            // If Length is not supported, do a conservative approach by not throwing here.
        }

        var uploadsFolder = Path.Combine(webRoot, folder);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // Ensure the stream is at the beginning before copying (some streams may be at the end)
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        // Use a unique file name to avoid collisions while preserving extension
        var extensionOnly = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extensionOnly}";

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = File.Create(filePath))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Return the public relative path that the client can use to access the file
        return $"/{folder}/{uniqueFileName}";
    }
}