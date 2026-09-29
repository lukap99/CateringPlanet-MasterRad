namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for an application user (admin user management + account profile).
/// </summary>
public class UserDto
{
    /// <summary>Identity user id (GUID string).</summary>
    public string Id { get; set; } = null!;

    /// <summary>User's first name.</summary>
    public string FirstName { get; set; } = null!;

    /// <summary>User's last name.</summary>
    public string LastName { get; set; } = null!;

    /// <summary>User's e-mail address (also the login name).</summary>
    public string Email { get; set; } = null!;

    /// <summary>Current loyalty points balance.</summary>
    public int Points { get; set; }

    /// <summary>Contact phone (Identity PhoneNumber), optional.</summary>
    public string? Phone { get; set; }

    /// <summary>Delivery address, optional.</summary>
    public string? Address { get; set; }

    /// <summary>Primary role (e.g. "Admin", "Manager", "Customer"), if known.</summary>
    public string? Role { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
