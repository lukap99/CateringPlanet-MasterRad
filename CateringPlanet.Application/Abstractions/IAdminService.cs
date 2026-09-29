using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Admin user-management operations (backed by ASP.NET Core Identity).
/// </summary>
public interface IAdminService
{
    /// <summary>Gets all users with their primary role.</summary>
    Task<IEnumerable<UserDto>> GetUsersAsync();

    /// <summary>Gets all users in a specific role.</summary>
    Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string role);

    /// <summary>Creates a user with the given role.</summary>
    Task<(bool Succeeded, string? Error)> CreateUserAsync(string email, string password, string firstName, string lastName, string role);

    /// <summary>Updates a user's name and optionally reassigns their role.</summary>
    Task<(bool Succeeded, string? Error)> UpdateUserAsync(string userId, string firstName, string lastName, string? role);

    /// <summary>Deletes a user.</summary>
    Task<(bool Succeeded, string? Error)> DeleteUserAsync(string userId);
}
