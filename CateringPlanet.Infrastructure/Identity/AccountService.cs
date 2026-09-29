using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using CateringPlanet.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace CateringPlanet.Infrastructure.Identity;

/// <summary>
/// Identity-backed implementation of <see cref="IAccountService"/>.
/// </summary>
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : await MapAsync(user);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : await MapAsync(user);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            result.Add(await MapAsync(user));
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<(bool Succeeded, string? Error)> RegisterAsync(string email, string password, string firstName, string lastName, string role, string? phone = null, string? address = null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            Address = address,
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
    public async Task<(bool Succeeded, string? Error)> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // Same message as a wrong password so the form does not leak which e-mails are registered.
            return (false, "Invalid username or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        return result.Succeeded
            ? (true, null)
            : (false, "Invalid username or password.");
    }

    /// <inheritdoc />
    public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return null;
        }

        // The API is stateless: clear the Identity cookie issued by PasswordSignInAsync
        // so the login only validates credentials and the client authenticates via JWT.
        await _signInManager.SignOutAsync();

        return await MapAsync(user);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? Enumerable.Empty<string>() : await _userManager.GetRolesAsync(user);
    }

    /// <inheritdoc />
    public async Task LogoutAsync()
        => await _signInManager.SignOutAsync();

    /// <inheritdoc />
    public async Task<(bool Succeeded, string? Error)> UpdateProfileAsync(string userId, string firstName, string lastName, string phone, string address)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return (false, "User not found.");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phone;
        user.Address = address;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    /// <inheritdoc />
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    /// <inheritdoc />
    public async Task<int> GetPointsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Points ?? 0;
    }

    /// <inheritdoc />
    public async Task AddPointsAsync(string userId, int delta)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return;
        }

        user.Points += delta;
        await _userManager.UpdateAsync(user);
    }

    private async Task<UserDto> MapAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Points = user.Points,
            Phone = user.PhoneNumber,
            Address = user.Address,
            Role = roles.FirstOrDefault()
        };
    }
}
