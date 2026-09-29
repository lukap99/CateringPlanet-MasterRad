using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Account/profile operations for application users (backed by ASP.NET Core Identity).
/// </summary>
public interface IAccountService
{
    /// <summary>Gets a user by id, or <c>null</c>.</summary>
    Task<UserDto?> GetByIdAsync(string userId);

    /// <summary>Gets a user by email, or <c>null</c>.</summary>
    Task<UserDto?> GetByEmailAsync(string email);

    /// <summary>Gets all users.</summary>
    Task<IEnumerable<UserDto>> GetAllAsync();

    /// <summary>Registers a new user with the given role. Phone and address are optional (default null).</summary>
    Task<(bool Succeeded, string? Error)> RegisterAsync(string email, string password, string firstName, string lastName, string role, string? phone = null, string? address = null);

    /// <summary>Signs a user in by e-mail/password (issues the Identity cookie).</summary>
    Task<(bool Succeeded, string? Error)> LoginAsync(string email, string password, bool rememberMe);

    /// <summary>
    /// Validates e-mail/password credentials without issuing a sign-in cookie (API login).
    /// Returns the user when the credentials are correct, otherwise <c>null</c>.
    /// </summary>
    Task<UserDto?> ValidateCredentialsAsync(string email, string password);

    /// <summary>Gets the user's roles (e.g. "Admin", "Manager", "Customer").</summary>
    Task<IEnumerable<string>> GetRolesAsync(string userId);

    /// <summary>Signs the current user out.</summary>
    Task LogoutAsync();

    /// <summary>Updates a user's first/last name, phone number, and address.</summary>
    Task<(bool Succeeded, string? Error)> UpdateProfileAsync(string userId, string firstName, string lastName, string phone, string address);

    /// <summary>Checks whether a user belongs to a role.</summary>
    Task<bool> IsInRoleAsync(string userId, string role);

    /// <summary>Gets the user's loyalty points balance.</summary>
    Task<int> GetPointsAsync(string userId);

    /// <summary>Adds (positive or negative) points to a user's balance.</summary>
    Task AddPointsAsync(string userId, int delta);
}
