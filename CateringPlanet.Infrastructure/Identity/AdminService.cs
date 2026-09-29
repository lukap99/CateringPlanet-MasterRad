using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using CateringPlanet.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace CateringPlanet.Infrastructure.Identity;

/// <summary>
/// Identity-backed implementation of <see cref="IAdminService"/> for admin user management.
/// </summary>
public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(Map(user, roles.FirstOrDefault()));
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Select(u => Map(u, role)).ToList();
    }

    /// <inheritdoc />
    public async Task<(bool Succeeded, string? Error)> CreateUserAsync(string email, string password, string firstName, string lastName, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            Points = 0
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, role);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool Succeeded, string? Error)> UpdateUserAsync(string userId, string firstName, string lastName, string? role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return (false, "User not found.");
        }

        user.FirstName = firstName;
        user.LastName = lastName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (role is not null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool Succeeded, string? Error)> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return (false, "User not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    private static UserDto Map(ApplicationUser user, string? role) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email ?? string.Empty,
        Points = user.Points,
        Role = role
    };
}
