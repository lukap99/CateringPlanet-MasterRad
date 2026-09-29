using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Stores, moves, and deletes uploaded images for menu items.
/// Implemented against the web root in the web layer; the application layer
/// only depends on this abstraction so storage can later be swapped (e.g. blob storage).
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Validates and saves an image stream into <c>images/{subfolder}</c>.
    /// The stored filename is human-readable: slug(<paramref name="baseName"/>) + "-" + GUID fragment + extension.
    /// </summary>
    /// <param name="content">The uploaded file stream.</param>
    /// <param name="baseName">Display name used to build the readable filename prefix (e.g. food name).</param>
    /// <param name="fileExtension">Original extension, used for whitelist validation.</param>
    /// <param name="contentLength">Uploaded size in bytes, checked against the 15 MB limit.</param>
    /// <param name="subfolder">Target subfolder under <c>images</c> (e.g. "food/supe-i-corbe" or "combo").</param>
    Task<ImageSaveResult> SaveAsync(Stream content, string baseName, string fileExtension, long contentLength, string subfolder);

    /// <summary>
    /// Deletes a stored image if it exists. Returns true when a file was deleted.
    /// </summary>
    /// <param name="fileName">Stored filename (not a path).</param>
    /// <param name="subfolder">Subfolder under <c>images</c> where the file lives.</param>
    Task<bool> DeleteAsync(string fileName, string subfolder);

    /// <summary>
    /// Moves a stored image from one subfolder to another (used when a food changes category).
    /// </summary>
    /// <param name="fileName">Stored filename (not a path).</param>
    /// <param name="sourceSubfolder">Current subfolder under <c>images</c>.</param>
    /// <param name="targetSubfolder">Destination subfolder under <c>images</c>.</param>
    Task MoveAsync(string fileName, string sourceSubfolder, string targetSubfolder);
}
