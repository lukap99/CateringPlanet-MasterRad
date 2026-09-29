namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Outcome of an image-save operation performed by <c>IImageStorageService</c>.
/// </summary>
public class ImageSaveResult
{
    /// <summary>Whether the image was validated and written to storage.</summary>
    public bool Success { get; set; }

    /// <summary>Stored filename (set when <see cref="Success"/> is true).</summary>
    public string? FileName { get; set; }

    /// <summary>Validation error message (set when <see cref="Success"/> is false).</summary>
    public string? Error { get; set; }
}
