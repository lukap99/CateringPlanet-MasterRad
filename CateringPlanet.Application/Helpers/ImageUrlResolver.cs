namespace CateringPlanet.Application.Helpers;

/// <summary>
/// Single source of truth for composing image URLs from stored filenames.
/// The database stores only the filename (e.g. "juneca_supa.jpg"); the folder path
/// is composed here at runtime so the storage layout can change without touching data.
/// </summary>
public static class ImageUrlResolver
{
    /// <summary>Fallback filename used when an item has no image.</summary>
    public const string DefaultImage = "no_image.png";

    /// <summary>Default base path when none is supplied by configuration.</summary>
    public const string DefaultBasePath = "/images";

    /// <summary>
    /// Builds the URL for a food image, scoped by category slug.
    /// </summary>
    /// <param name="fileName">Stored filename (may be null).</param>
    /// <param name="categorySlug">URL-safe category slug (e.g. "supe-i-corbe").</param>
    /// <param name="basePath">Base path read from configuration; defaults to "/images".</param>
    public static string ForFood(string? fileName, string categorySlug, string basePath = DefaultBasePath)
        => fileName is null
            ? $"{Normalize(basePath)}/{DefaultImage}" // /images/no_image.png
            : $"{Normalize(basePath)}/food/{categorySlug}/{fileName}";

    /// <summary>
    /// Builds the URL for a combo menu image.
    /// </summary>
    /// <param name="fileName">Stored filename (may be null).</param>
    /// <param name="basePath">Base path read from configuration; defaults to "/images".</param>
    public static string ForCombo(string? fileName, string basePath = DefaultBasePath)
        => fileName is null
            ? $"{Normalize(basePath)}/{DefaultImage}" // /images/no_image.png
            : $"{Normalize(basePath)}/combo/{fileName}";

    private static string Normalize(string basePath) => basePath.TrimEnd('/');
}
