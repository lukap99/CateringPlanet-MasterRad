using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Helpers;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace CateringPlanet.Web.Adapters;

/// <summary>
/// Stores menu images on the local file system under <c>wwwroot/images/{subfolder}</c>.
/// Validates extension against an allow-list (no SVG) and enforces a 15 MB size cap,
/// streaming the upload directly to disk without buffering it in memory.
/// </summary>
public class LocalImageStorageService : IImageStorageService
{
    /// <summary>Maximum accepted upload size in bytes (15 MB).</summary>
    public const long MaxImageBytes = 15 * 1024 * 1024;

    /// <summary>Allowed image extensions (lowercase, case-insensitive comparison).</summary>
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly IWebHostEnvironment _env;

    public LocalImageStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <inheritdoc />
    public async Task<ImageSaveResult> SaveAsync(Stream content, string baseName, string fileExtension, long contentLength, string subfolder)
    {
        var extension = fileExtension?.ToLowerInvariant() ?? string.Empty;
        if (!AllowedExtensions.Contains(extension))
        {
            return new ImageSaveResult { Success = false, Error = "Unsupported file type. Allowed: JPG, JPEG, PNG, GIF, WEBP." };
        }

        if (contentLength > MaxImageBytes)
        {
            return new ImageSaveResult { Success = false, Error = "Image must be 15 MB or smaller." };
        }

        // Human-readable filename: slug(name)-xxxxxxxx.ext; fall back to the GUID fragment alone.
        var slug = string.IsNullOrWhiteSpace(baseName) ? string.Empty : SlugHelper.Slugify(baseName);
        var prefix = slug.Length > 0 ? slug + "-" : string.Empty;
        var fileName = $"{prefix}{Guid.NewGuid().ToString("N")[..8]}{extension}";

        var dir = Path.Combine(_env.WebRootPath, "images", subfolder);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        // Stream straight to disk; never buffer the upload in memory.
        await using (var stream = new FileStream(path, FileMode.Create))
        {
            await content.CopyToAsync(stream);
        }

        return new ImageSaveResult { Success = true, FileName = fileName };
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(string fileName, string subfolder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.FromResult(false);
        }

        var path = Path.Combine(_env.WebRootPath, "images", subfolder, fileName);
        if (!File.Exists(path))
        {
            return Task.FromResult(false);
        }

        File.Delete(path);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task MoveAsync(string fileName, string sourceSubfolder, string targetSubfolder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.CompletedTask;
        }

        var sourcePath = Path.Combine(_env.WebRootPath, "images", sourceSubfolder, fileName);
        if (!File.Exists(sourcePath))
        {
            return Task.CompletedTask;
        }

        var targetDir = Path.Combine(_env.WebRootPath, "images", targetSubfolder);
        Directory.CreateDirectory(targetDir);
        File.Move(sourcePath, Path.Combine(targetDir, fileName));
        return Task.CompletedTask;
    }
}
