namespace CateringPlanet.Application.Helpers;

/// <summary>
/// Shared URL-slug helpers used by menu services and image storage.
/// </summary>
public static class SlugHelper
{
    /// <summary>
    /// Builds a URL-safe slug from a display name: lowercases, strips diacritics
    /// (č ć š ž đ → c c s z d), and replaces every other non-alphanumeric character with '-'.
    /// </summary>
    public static string Slugify(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = slug.Replace('č', 'c').Replace('ć', 'c').Replace('š', 's').Replace('ž', 'z').Replace('đ', 'd');
        var chars = slug.Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        return new string(chars);
    }
}
